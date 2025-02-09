using System.Diagnostics;
using Checkpoint_Manager.Models;
using CommunityToolkit.Mvvm.Input;

namespace Checkpoint_Manager.ViewModels
{
    public class AddSaveViewModel{
        public string? Name { get; set; }
        public string? Description { get; set; }

        public RelayCommand ConfirmCommand { get; }
        public Action? CloseAction { get; set; }

        public AddSaveViewModel() {
            ConfirmCommand = new RelayCommand(Confirm);
        }

        private void Confirm() {
            if (!String.IsNullOrEmpty(Name) && NameValidator.IsValidName(Name)) {
                string newSaveId = IdGetter.CreateId(Name);
                Game? selectedGame = App.MainViewModelInstance.SelectedGame;

                if (selectedGame != null) {
                    foreach (Save cadastredSave in selectedGame.Saves) {
                        #pragma warning disable CS8602
                        if (cadastredSave.Id.Equals(newSaveId)) {
                            System.Windows.Forms.MessageBox.Show("Um Checkpoint com esse nome já foi cadastrado.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        #pragma warning restore CS8602
                    }

                    Debug.WriteLine("Chamada do NewSave pela janela");
                    if (!selectedGame.NewSave(Name, Description)) {
                        Debug.WriteLine("Erro ao criar save personalizado");
                    } else {
                        FileManager.AttArquives(App.MainViewModelInstance.Games);
                        CloseAction?.Invoke();
                    }
                }

            } else {
                System.Windows.Forms.MessageBox.Show("Nome inválido!", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
