using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HustleCastLoot
{
    public class PersonItemViewModel : ViewModelBase
    {
        public const int DiamondIndex = -1;
        public const int TicketsIndex = -2;
        public const int ShardsIndex = -3;
        public const int DustIndex = -4;

        internal static List<PersonItemViewModel> BuildDefaultList(int count)
        {
            var result = new List<PersonItemViewModel>();

            result.Add(new PersonItemViewModel(DiamondIndex));
            result.Add(new PersonItemViewModel(TicketsIndex));
            result.Add(new PersonItemViewModel(ShardsIndex));
            result.Add(new PersonItemViewModel(DustIndex));

            for (int i = 1; i < count; i++)
            {
                result.Add(new PersonItemViewModel(i));
            }

            return result;
        }

        internal static string GetDisplay(int index)
        {
            if (CustomDisplaies.ContainsKey(index) && !string.IsNullOrEmpty(CustomDisplaies[index]))
            {
                return CustomDisplaies[index];
            }
            else
            {
                switch (index)
                {
                    case DiamondIndex:
                        return "Diamonds";
                    case TicketsIndex:
                        return "Tickets";
                    case ShardsIndex:
                        return "Shards";
                    case DustIndex:
                        return "Dust";
                    default:
                        return $"Item #{index}";
                }
            }
        }

        private static Dictionary<int, string> CustomDisplaies { get; } = new Dictionary<int, string>();

        public override void Execute(object parameter)
        {
            switch (parameter)
            {
                case "EditCustomDisplay":
                    EditVisible = Visibility.Visible;
                    EditNotVisible = Visibility.Collapsed;
                    break;
                case "SaveEdit":
                    if (CustomDisplaies.ContainsKey(Index))
                    {
                        CustomDisplaies[Index] = EditCustomDisplay;
                    }
                    else
                    {
                        CustomDisplaies.Add(Index, EditCustomDisplay);
                    }

                    EditVisible = Visibility.Collapsed;
                    EditNotVisible = Visibility.Visible;
                    RaisePropertyChanged("Display");
                    break;
            }
        }

        public string Display { get { return GetDisplay(Index); } }

        public int Index { get; }

        public PersonItemViewModel(int index)
        {
            Index = index;

            EditVisible = Visibility.Collapsed;
            EditNotVisible = Visibility.Visible;
        }

        public Visibility EditNotVisible
        {
            get { return GetValue<Visibility>(); }
            set { SetValue(value); }
        }

        public Visibility EditVisible
        {
            get { return GetValue<Visibility>(); }
            set { SetValue(value); }
        }

        public string EditCustomDisplay
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public bool IsWon
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool IsHighGlory
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool IsNeed
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool IsCanUse
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }
    }
}
