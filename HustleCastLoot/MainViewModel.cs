using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HustleCastLoot
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            DiamondRolls = 3;
            TicketRolls = 0;
            ShardRolls = 3;
            DustRolls = 3;
        }

        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "Add": AddPerson(); break;
                case "Save": SavePeople(); break;
                case "Load": LoadPeople(); break;
                case "DoRolls": DoRolls(); break;
                case "ResetWins": DoResetWin(); break;
                case "ResetLoot": DoResetLoot(); break;
            }
        }

        public ObservableCollection<PersonViewModel> People { get; } = new ObservableCollection<PersonViewModel>();

        public List<ItemRollViewModel> ItemRollList
        {
            get { return GetValue<List<ItemRollViewModel>>(); }
            set { SetValue(value); }
        }

        public int DiamondRolls
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int TicketRolls
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int ShardRolls
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int DustRolls
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        private void AddPerson()
        {
            var newPerson = new PersonViewModel();
            newPerson.RemoveClicked += (p) => People.Remove(p);
            People.Add(newPerson);
        }

        private const string PeopleSaveFileName = "PeopleSave.txt";

        private void SavePeople()
        {
            var text = People.Select(x => x.GetSaveLine()).ToArray();
            File.WriteAllLines(PeopleSaveFileName, text);
        }

        private void LoadPeople()
        {
            if (File.Exists(PeopleSaveFileName))
            {
                var lines = File.ReadAllLines(PeopleSaveFileName);
                People.Clear();

                foreach (var line in lines.OrderBy(x => x))
                {
                    var newPerson = new PersonViewModel(line);
                    newPerson.RemoveClicked += (p) => People.Remove(p);
                    People.Add(newPerson);
                }
            }
        }

        private void DoRolls()
        {
            ItemRollList = ItemRollViewModel.DoRolls(People.Where(x => !x.IsLowGlory).ToList(), DiamondRolls, TicketRolls, ShardRolls, DustRolls);
        }

        private void DoResetWin()
        {
            foreach (var person in People)
            {
                person.ResetWins();
            }
        }

        private void DoResetLoot()
        {
            foreach (var person in People)
            {
                person.ResetLootPicks();
            }
        }
    }
}
