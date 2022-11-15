using System.Net.Http.Json;
using Blazored.Modal.Services;
using CRMApp.Auth;
using CRMWebClient.Common;
using CRMWebClient.Shared;
using Microsoft.AspNetCore.Components;
using CRMAppEntity;
using System.Text.Json;
using System.Reflection;
using Blazored.Modal;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using Microsoft.JSInterop;
using System.Collections.Generic;

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

          await  SearchData();
        }
        private IJSObjectReference module;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import",
                    "./script/customer.js");

                await module.InvokeVoidAsync("init");
            }
        }

        private async Task SearchData()
        {
  
            var modal = ModalService.Show<Loading>("", SharedModalOptions.modalOptionsWait);

            Customer customer = new Customer() { Name = this.CustomerName, Address = this.Address };
            var result = await Http.PostAsJsonAsync<Customer>("https://localhost:7197/api/Customer/GetCustomers", customer);

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
                    customerList =reponseContent.Content;                 
                    modal.Close();
                    break;
                default: //其它服务器错误
                    ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo(reponseContent.Msg, "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                    modal.Close();
                    break;
            }
        }

        
            public async Task SearchClick()
        {
            await SearchData();
            StateHasChanged();
        }
        public async Task AddClick()
        {
         
           

            Customer customer = new ();
            var parameters = new ModalParameters();
            parameters.Add(nameof(Edit.customer), customer);
            parameters.Add(nameof(Edit.actionType), 1); //1 add 2 edit 3 detail
           

            
            var editForm = ModalService.Show<Edit>("Edit" ,parameters);
            var result = await editForm.Result;

            if (result.Confirmed)
            {
                await SearchData();
                StateHasChanged();

            }
        }

        
        public async Task EditClick()
        {
            string jsValue =  await module.InvokeAsync<string>("getChkValue");
            string[] selIds = jsValue.Split(",");
            if (selIds.Length==0)
            {
                ModalService.Show<ModalInfo>("Info", SharedModalOptions.SetParameterModalInfo("Please select!", "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                return;
            }

            Customer customer = customerList.FirstOrDefault<Customer>(e=>e.Id == Convert.ToInt32(selIds[0]));
            var parameters = new ModalParameters();
            parameters.Add(nameof(Edit.customer), customer);
            parameters.Add(nameof(Edit.actionType), 2); //1 add 2 edit 3 detail
            
            var editForm = ModalService.Show<Edit>("Edit", parameters);
            var result = await editForm.Result;

            if (result.Confirmed)
            {
                await SearchData();
                StateHasChanged();

            }
        }
        public async Task DelClick()
        {
            string jsValue = await module.InvokeAsync<string>("getChkValue");
            string[] selIds = jsValue.Split(",");
            if (selIds.Length == 0)
            {
                ModalService.Show<ModalInfo>("Info", SharedModalOptions.SetParameterModalInfo("Please select!", "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                return;
            }

            var modal = ModalService.Show<Loading>("", SharedModalOptions.modalOptionsWait);
            try
            {
                 
                HttpResponseMessage result = await Http.DeleteAsync("https://localhost:7197/api/Customer/DelCustomer/"+ string.Join(',', selIds));


                if (result == null)
                {
                    ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo("Server error!", "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                    return;
                }

                JsonTemplate<string> reponseContent = JsonSerializer.Deserialize<JsonTemplate<string>>(result.Content.ReadAsStringAsync().Result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                switch (reponseContent.StatusCode)
                {
                    case "200": //成功                                          
                        await module.InvokeVoidAsync("delRow");
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
