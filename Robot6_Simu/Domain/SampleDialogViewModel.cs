namespace Robot6_Simu.Domain
{
    public class SampleDialogViewModel : ViewModelBase
    {
        private string? _name;

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}