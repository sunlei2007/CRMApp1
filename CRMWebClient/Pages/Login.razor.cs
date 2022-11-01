using Blazored.Modal.Services;
using CRMWebClient.Common;
using CRMWebClient.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace CRMWebClient.Pages
{
    public partial class Login
    {
        [CascadingParameter] protected IModalService ModalService { get; set; }

        private string UserName { get; set; }
        private string Password { get; set; }
        private async void OnClick()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return;
            }
            var modal = ModalService.Show<Loading>("", SharedModalOptions.modalOptionsWait);

            JsonTemplate<string> result = await Http.GetFromJsonAsync<JsonTemplate<string>>($"https://localhost:7197/api/Login/Login/{UserName}/{Password}");

            if (result == null)
            {
                modal.Close();
                ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo("Server error!", "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                return;
            }
            switch (result.StatusCode)
            {
                case "200": //成功
                    GlobalVariable.UserName = UserName;
                    await loginService.Login(result.Content);
                    modal.Close();
                    break;
                case "301": //用户或密码不存在
                    modal.Close();
                    ModalService.Show<ModalInfo>("Error", SharedModalOptions.SetParameterModalInfo(result.Msg, "alert alert-danger"), SharedModalOptions.modalOptionsInfo);
                    break;
                default: //其它服务器错误
                    modal.Close();
                    break;
            }
            if (result.StatusCode == "200")
            {

                GlobalVariable.UserName = UserName;
                await loginService.Login(result.Content);
                modal.Close();
            }
        }
    }
}
