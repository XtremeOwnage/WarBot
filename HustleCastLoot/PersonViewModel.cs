using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustleCastLoot
{
    public class PersonViewModel : ViewModelBase
    {
        public PersonViewModel()
        {

        }

        public PersonViewModel(string source)
        {
            var entries = source.Split('|');

            var name = entries.FirstOrDefault();
            var won = entries.Skip(1).FirstOrDefault();
            var highGlory = entries.Skip(2).FirstOrDefault();
            var need = entries.Skip(3).FirstOrDefault();
            var canUse = entries.Skip(4).FirstOrDefault();
            var lowGlory = entries.Skip(5).FirstOrDefault();

            if (name != null)
            {
                Name = name;
            }

            if (!string.IsNullOrEmpty(won))
            {
                foreach (var item in won.Split(','))
                {
                    int itemInt;

                    if (int.TryParse(item, out itemInt))
                    {
                        var itemVm = ItemList.SingleOrDefault(x => x.Index == itemInt);
                        if (itemVm != null)
                            itemVm.IsWon = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(highGlory))
            {
                foreach (var item in highGlory.Split(','))
                {
                    int itemInt;

                    if (int.TryParse(item, out itemInt))
                    {
                        var itemVm = ItemList.SingleOrDefault(x => x.Index == itemInt);
                        if (itemVm != null)
                            itemVm.IsHighGlory = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(need))
            {
                foreach (var item in need.Split(','))
                {
                    int itemInt;

                    if (int.TryParse(item, out itemInt))
                    {
                        var itemVm = ItemList.SingleOrDefault(x => x.Index == itemInt);
                        if (itemVm != null)
                            itemVm.IsNeed = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(canUse))
            {
                foreach (var item in canUse.Split(','))
                {
                    int itemInt;

                    if (int.TryParse(item, out itemInt))
                    {
                        var itemVm = ItemList.SingleOrDefault(x => x.Index == itemInt);
                        if (itemVm != null)
                            itemVm.IsCanUse = true;
                    }
                }
            }

            var isLowGlory = false;

            if (bool.TryParse(lowGlory ?? string.Empty, out isLowGlory))
            {
                IsLowGlory = isLowGlory;
            }
        }

        public string GetSaveLine()
        {
            var wonItemList = ItemList.Where(x => x.IsWon).Select(x => x.Index);
            var gloryItemList = ItemList.Where(x => x.IsHighGlory).Select(x => x.Index);
            var needItemList = ItemList.Where(x => x.IsNeed).Select(x => x.Index);
            var canUseItemList = ItemList.Where(x => x.IsCanUse).Select(x => x.Index);

            return $"{Name}|{string.Join(",", wonItemList)}|{string.Join(",", gloryItemList)}|{string.Join(",", needItemList)}|{string.Join(",", canUseItemList)}|{IsLowGlory}";
        }

        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "Remove":
                    RemoveClicked?.Invoke(this);
                    break;
            }
        }

        internal void ResetWins()
        {
            IsLowGlory = false;

            ItemList.ForEach(x =>
            {
                x.IsWon = false;
            });
        }

        internal void ResetLootPicks()
        {
            ItemList.ForEach(x =>
            {
                x.IsHighGlory = false;
                x.IsNeed = false;
                x.IsCanUse = false;
            });
        }

        public event Action<PersonViewModel> RemoveClicked;

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public bool IsLowGlory
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public List<int> GetItemsWon()
        {
            return ItemList.Where(x => x.IsWon).Select(x => x.Index).ToList();
        }

        public List<PersonItemViewModel> ItemList { get; } = PersonItemViewModel.BuildDefaultList(50);
    }
}
