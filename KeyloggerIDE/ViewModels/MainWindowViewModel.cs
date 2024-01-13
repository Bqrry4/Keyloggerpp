using ReactiveUI;

namespace KeyloggerIDE.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _contentViewModel;

        private ViewModelBase _secondViewModel;

        //this has a dependency on the ToDoListService

        public MainWindowViewModel()
        {
            _contentViewModel = new MainViewModel();
            _secondViewModel = new SettingsViewModel();
        }

        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            private set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }

        public void ChangeViewModel()
        {
            var tmp = _contentViewModel;
            ContentViewModel = _secondViewModel;
            _secondViewModel = tmp;
        }
    }
}
