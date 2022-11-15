using Blazored.Modal.Services;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using CRMWebClient.Shared;
using System.Text.Json;
using CRMAppEntity;
using System.Net.Http.Json;
using CRMWebClient.Common;
using System.Reflection;

namespace CRMWebClient.Pages.customer
{
    public partial class Edit
    {
        [Parameter] public Customer? customer { get; set; }
        [Parameter] public int actionType { get; set; }
        
        [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;
        [CascadingParameter] protected IModalService ModalService { get; set; }

   
        private async Task Cancel() => await BlazoredModal.CancelAsync();

        private async Task SubmitForm()
        {
            switch (actionType)
            {
                case 1 or 2:
                   await AddData(actionType);
                   break;
            }
        }
        private async Task AddData(int actionType)
        {
            var modal = ModalService.Show<Loading>("", SharedModalOptions.modalOptionsWait);
            try
            {
                HttpResponseMessage result=null;

                if (actionType == 1)
                    result = await Http.PostAsJsonAsync<Customer>("https://localhost:7197/api/Customer/AddCustomer", customer);
                if (actionType == 2)
                    result = await Http.PutAsJsonAsync<Customer>("https://localhost:7197/api/Customer/EditCustomer", customer);


                if (result == null)
                {            
                    ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo("Server error!", "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                    return;
                }

                JsonTemplate<string> reponseContent = JsonSerializer.Deserialize<JsonTemplate<string>>(result.Content.ReadAsStringAsync().Result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                switch (reponseContent.StatusCode)
                {
                    case "200": //成功                                          
                        await BlazoredModal.CloseAsync(ModalResult.Ok());
                        break;
                    default: //其它服务器错误
                        ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo(reponseContent.Msg, "alert alert-danger"), SharedModalOptions.modalOptionsInfo);                     
                        break;
                }
            }
            catch
            {
                ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo("Server error!", "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
            }
            finally
            {
                modal.Close();
            }
        }


    }
}
