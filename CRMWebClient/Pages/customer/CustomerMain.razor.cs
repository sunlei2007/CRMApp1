using System.Net.Http.Json;
using Blazored.Modal.Services;
using CRMApp.Auth;
using CRMWebClient.Common;
using CRMWebClient.Shared;
using Microsoft.AspNetCore.Components;
using CRMAppEntity;
using System.Text.Json;

namespace CRMWebClient.Pages.customer
{
    public partial class CustomerMain
    {
        [CascadingParameter] protected IModalService ModalService { get; set; }

        private string CustomerName { get; set; } = "";
        private string Address { get; set; } = "";

        private List<Customer>? customerList;
        protected override async Task OnInitializedAsync()
        {
     
            var modal = ModalService.Show<Loading>("", SharedModalOptions.modalOptionsWait);

            Customer customer = new Customer() { Name = this.CustomerName, Address = this.Address };
           var result = await Http.PostAsJsonAsync<Customer>("https://localhost:7197/api/Customer/GetCustomers", customer) ;

            if (result == null)
            {
                modal.Close();
                ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo("Server error!", "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                return;
            }

            JsonTemplate<List<Customer>> reponseContent = JsonSerializer.Deserialize<JsonTemplate<List<Customer>>>(result.Content.ReadAsStringAsync().Result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            switch (reponseContent.StatusCode)
            {
                case "200": //成功
                    customerList=reponseContent.Content;
                    modal.Close();
                    break;
                default: //其它服务器错误
                    ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo(reponseContent.Msg, "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                    modal.Close();
                    break;
            }
        }
    }
}
