using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustleCastLoot
{
    public class ItemRollViewModel : ViewModelBase
    {
        private static Random Rng { get; } = new Random();

        public void DoLootRoll(int itemNumber, List<PersonViewModel> rollers, List<PersonViewModel> nonRollers, RollType rollType)
        {
            ItemNumber = itemNumber;
            var itemDisplay = PersonItemViewModel.GetDisplay(itemNumber);
            var rollOutput = new StringBuilder();

            rollOutput.AppendLine($"*** Roll for item {itemDisplay}.  This is a [{rollType}] roll. ***");

            if (rollers.Count == 0)
            {
                rollOutput.AppendLine("No Rollers");
            }
            else
            {
                var rolls = new Dictionary<PersonViewModel, int>();

                foreach (var nonRoller in nonRollers)
                {
                    rollOutput.AppendLine($"{nonRoller.Name} can't roll for this because they have won {nonRoller.GetItemsWon().Count} item(s) so far");
                }

                foreach (var roller in rollers.OrderBy(x => Rng.Next(1, 10000000)))
                {
                    rolls.Add(roller, Rng.Next(1, 10001));
                }

                foreach (var roll in rolls.OrderByDescending(x => x.Value))
                {
                    if (Winner == null)
                    {
                        rollOutput.AppendLine($"{roll.Key.Name} (Won {roll.Key.GetItemsWon().Count} so far) : {rolls[roll.Key]} WINNER!");
                        roll.Key.ItemList.Single(x => x.Index == ItemNumber).IsWon = true;
                        Winner = roll.Key;
                    }
                    else
                    {
                        rollOutput.AppendLine($"{roll.Key.Name} (Won {roll.Key.GetItemsWon().Count} so far) : {rolls[roll.Key]}");
                    }
                }
            }

            if (Winner == null)
            {
                Header = $"{itemDisplay} - (NO ROLLERS)";
            }
            else 
            {
                Header = $"{itemDisplay} - {Winner.Name}";
            }

            RollOutput = rollOutput.ToString();
        }

        public void DoLottoRoll(List<PersonViewModel> lottoRollers)
        {
            Header = "Lotto Leftovers";
            var orderOutput = new StringBuilder();
            orderOutput.AppendLine("Left over Lotto RNG order");
            GetLottoOrder(orderOutput, lottoRollers, 0);
            RollOutput = orderOutput.ToString();
        }

        public void DoHighGlory(Dictionary<int, List<PersonViewModel>> highGloryDict)
        {
            Header = "High Glory";
            var orderOutput = new StringBuilder();
            orderOutput.AppendLine("High Glory Picks");

            foreach (var highGloryItem in highGloryDict)
            {
                foreach (var person in highGloryItem.Value)
                {
                    foreach (var item in person.ItemList.Where(x => x.IsHighGlory))
                    {
                        orderOutput.AppendLine($"{person.Name} - {PersonItemViewModel.GetDisplay(highGloryItem.Key)}");
                        item.IsWon = true;
                    }
                }
            }

            RollOutput = orderOutput.ToString();
        }

        private void GetLottoOrder(StringBuilder orderOutput, List<PersonViewModel> rollers, int index)
        {
            if (rollers.Count == 0)
                return;

            var validPeople = rollers.Where(p => p.ItemList.Count(i => i.IsWon) <= index).Select(x => new
            {
                Roll = Rng.Next(1, 10001),
                Person = x
            }).OrderByDescending(x => x.Roll).ToList();
            var invalidPeople = rollers.Where(p => p.ItemList.Count(i => i.IsWon) > index).ToList();

            orderOutput.AppendLine($"*** People that won {index} item(s) in round 1 and 2 ***");

            foreach (var person in validPeople)
            {
                orderOutput.AppendLine($"{person.Person.Name} - {person.Roll}");
            }

            GetLottoOrder(orderOutput, invalidPeople, ++index);
        }

        public int ItemNumber
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Header
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string RollOutput
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public PersonViewModel Winner
        {
            get { return GetValue<PersonViewModel>(); }
            set { SetValue(value); }
        }

        internal static List<ItemRollViewModel> DoRolls(List<PersonViewModel> people, int diamondRolls, int ticketRolls, int shardRolls, int dustRolls)
        {
            var result = new List<ItemRollViewModel>();

            var itemsWon = new List<int>();
            var highGloryItems = new Dictionary<int, List<PersonViewModel>>();
            var needItems = new Dictionary<int, List<PersonViewModel>>();
            var canUseItems = new Dictionary<int, List<PersonViewModel>>();

            foreach (var person in people)
            {
                foreach (var item in person.ItemList)
                {
                    if (item.IsHighGlory)
                    {
                        if (!highGloryItems.ContainsKey(item.Index))
                        {
                            highGloryItems.Add(item.Index, new List<PersonViewModel>());
                        }

                        highGloryItems[item.Index].Add(person);
                    }

                    if (item.IsNeed)
                    {
                        if (!needItems.ContainsKey(item.Index))
                        {
                            needItems.Add(item.Index, new List<PersonViewModel>());
                        }

                        needItems[item.Index].Add(person);
                    }

                    if (item.IsCanUse)
                    {
                        if (!canUseItems.ContainsKey(item.Index))
                        {
                            canUseItems.Add(item.Index, new List<PersonViewModel>());
                        }

                        canUseItems[item.Index].Add(person);
                    }
                }
            }

            // high glory
            result.Add(new ItemRollViewModel());
            result.Last().DoHighGlory(highGloryItems);

            itemsWon = people.SelectMany(p => p.ItemList.Where(i => i.IsWon).Select(x => x.Index)).Distinct().ToList();
            needItems = needItems.Where(x => !itemsWon.Contains(x.Key) || x.Key < 0).ToDictionary(k => k.Key, v => v.Value);

            // need gear
            foreach (var item in needItems.Where(i => i.Key > 0).OrderBy(i => i.Value.Count()))
            {
                var needyPeople = GetNeedyRollers(item.Key, item.Value, 0);
                var nonRollers = item.Value.Where(p => !needyPeople.Contains(p)).ToList();
                result.Add(new ItemRollViewModel());
                result.Last().DoLootRoll(item.Key, needyPeople, nonRollers, RollType.Need);
            }

            // need diamonds
            if (needItems.ContainsKey(PersonItemViewModel.DiamondIndex))
            {
                while (diamondRolls > people.Count(p => p.ItemList.Single(x => x.Index == PersonItemViewModel.DiamondIndex).IsWon))
                {
                    var needyPeople = GetNeedyRollers(PersonItemViewModel.DiamondIndex, needItems[PersonItemViewModel.DiamondIndex], 0);
                    var nonRollers = needItems[PersonItemViewModel.DiamondIndex].Where(p => !needyPeople.Contains(p)).ToList();
                    result.Add(new ItemRollViewModel());
                    result.Last().DoLootRoll(PersonItemViewModel.DiamondIndex, needyPeople, nonRollers, RollType.Need);

                    needItems[PersonItemViewModel.DiamondIndex].Remove(result.Last().Winner);
                    if (needItems[PersonItemViewModel.DiamondIndex].Count == 0)
                    {
                        break;
                    }
                }
            }

            // need tickets
            if (needItems.ContainsKey(PersonItemViewModel.TicketsIndex))
            {
                while (ticketRolls > people.Count(p => p.ItemList.Single(x => x.Index == PersonItemViewModel.TicketsIndex).IsWon))
                {
                    var needyPeople = GetNeedyRollers(PersonItemViewModel.TicketsIndex, needItems[PersonItemViewModel.TicketsIndex], 0);
                    var nonRollers = needItems[PersonItemViewModel.TicketsIndex].Where(p => !needyPeople.Contains(p)).ToList();
                    result.Add(new ItemRollViewModel());
                    result.Last().DoLootRoll(PersonItemViewModel.TicketsIndex, needyPeople, nonRollers, RollType.Need);

                    needItems[PersonItemViewModel.TicketsIndex].Remove(result.Last().Winner);
                    if (needItems[PersonItemViewModel.TicketsIndex].Count == 0)
                    {
                        break;
                    }
                }
            }

            // need shards
            if (needItems.ContainsKey(PersonItemViewModel.ShardsIndex))
            {
                while (shardRolls > people.Count(p => p.ItemList.Single(x => x.Index == PersonItemViewModel.ShardsIndex).IsWon))
                {
                    var needyPeople = GetNeedyRollers(PersonItemViewModel.ShardsIndex, needItems[PersonItemViewModel.ShardsIndex], 0);
                    var nonRollers = needItems[PersonItemViewModel.ShardsIndex].Where(p => !needyPeople.Contains(p)).ToList();
                    result.Add(new ItemRollViewModel());
                    result.Last().DoLootRoll(PersonItemViewModel.ShardsIndex, needyPeople, nonRollers, RollType.Need);

                    needItems[PersonItemViewModel.ShardsIndex].Remove(result.Last().Winner);
                    if (needItems[PersonItemViewModel.ShardsIndex].Count == 0)
                    {
                        break;
                    }
                }
            }

            // need dust
            if (needItems.ContainsKey(PersonItemViewModel.DustIndex))
            {
                while (dustRolls > people.Count(p => p.ItemList.Single(x => x.Index == PersonItemViewModel.DustIndex).IsWon))
                {
                    var needyPeople = GetNeedyRollers(PersonItemViewModel.DustIndex, needItems[PersonItemViewModel.DustIndex], 0);
                    var nonRollers = needItems[PersonItemViewModel.DustIndex].Where(p => !needyPeople.Contains(p)).ToList();
                    result.Add(new ItemRollViewModel());
                    result.Last().DoLootRoll(PersonItemViewModel.DustIndex, needyPeople, nonRollers, RollType.Need);

                    needItems[PersonItemViewModel.DustIndex].Remove(result.Last().Winner);
                    if (needItems[PersonItemViewModel.DustIndex].Count == 0)
                    {
                        break;
                    }
                }
            }

            itemsWon = people.SelectMany(p => p.ItemList.Where(i => i.IsWon).Select(x => x.Index)).Distinct().ToList();
            canUseItems = canUseItems.Where(x => !itemsWon.Contains(x.Key)).ToDictionary(k => k.Key, v => v.Value);

            // can use
            foreach (var item in canUseItems)
            {
                var canUsePeople = GetCanUseRollers(item.Key, item.Value, 0);
                var nonRollers = item.Value.Where(p => !canUsePeople.Contains(p)).ToList();
                result.Add(new ItemRollViewModel());
                result.Last().DoLootRoll(item.Key, canUsePeople, nonRollers, RollType.CanUse);
            }
           
            // lotto leftovers
            result.Add(new ItemRollViewModel());
            result.Last().DoLottoRoll(people.Where(x => !x.IsLowGlory).ToList());

            // full output
            var fullOutput = string.Join(Environment.NewLine, result.Select(x => x.RollOutput));
            result.Add(new ItemRollViewModel());
            result.Last().Header = "Full Output";
            result.Last().RollOutput = fullOutput;

            return result;
        }

        private static List<PersonViewModel> GetNeedyRollers(int itemNumber, List<PersonViewModel> people, int winCount)
        {
            if (people.Count <= 1)
                return people;

            var rollers = people.Where(p => p.ItemList.Any(i => i.Index == itemNumber && i.IsNeed && p.GetItemsWon().Count <= winCount)).ToList();

            if (rollers.Count == 0)
            {
                return GetNeedyRollers(itemNumber, people, ++winCount);
            }
            else
            {
                return rollers;
            }
        }

        private static List<PersonViewModel> GetCanUseRollers(int itemNumber, List<PersonViewModel> people, int winCount)
        {
            if (people.Count <= 1)
                return people;

            var rollers = people.Where(p => p.ItemList.Any(i => i.Index == itemNumber && i.IsCanUse && p.GetItemsWon().Count <= winCount)).ToList();

            if (rollers.Count == 0)
            {
                return GetCanUseRollers(itemNumber, people, ++winCount);
            }
            else
            {
                return rollers;
            }
        }
    }
}
