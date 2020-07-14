using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using project_cad.Models;
using project_cad.Views;
using System.Collections.Generic;

namespace project_cad.ViewModels
{
    public class DocumentsViewModel : BaseViewModel
    {
        public List<Document> DataStore = new List<Document>();

        public ObservableCollection<Document> Documents { get; set; }
        public Command LoadDocumentsCommand { get; set; }

        public DocumentsViewModel()
        {
            Title = "Browse";
            Documents = new ObservableCollection<Document>();
            LoadDocumentsCommand = new Command(async () => await ExecuteLoadDocumentsCommand());

            MessagingCenter.Subscribe<NewDocumentPage, Document>(this, "AddDocument", async (obj, document) =>
            {
                var newDocument = document as Document;

                Documents.Add(newDocument);

                await Task.Run(() => DataStore.Add(newDocument));
            });
        }

        async Task ExecuteLoadDocumentsCommand() // TODO: async
        {
            IsBusy = true;

            try
            {
                Documents.Clear();

                foreach (var document in DataStore)
                {
                    Documents.Add(document);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}