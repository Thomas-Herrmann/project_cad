using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ProjectCadLanguage;

namespace project_cad.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = Language.valueToString(Language.interpret(Language.exampleEnv, Language.exampleAST7));
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
        }

        public ICommand OpenWebCommand { get; }
    }
}