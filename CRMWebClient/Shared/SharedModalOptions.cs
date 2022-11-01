using Blazored.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMWebClient.Shared
{
    public class SharedModalOptions
    {
        public static ModalOptions modalOptionsWait = new ModalOptions()
        {
            Class = "dialogmodal-wait",
            Position = ModalPosition.Middle,
            //Size=ModalSize.Automatic,
            DisableBackgroundCancel = true,
            HideCloseButton = true,
            HideHeader = true
            
        };

        public static ModalOptions modalOptionsInfo = new ModalOptions()
        {
            DisableBackgroundCancel = true,
            AnimationType = ModalAnimationType.FadeInOut
        };


        public static ModalParameters SetParameterModalInfo(string msj, string cssClass)
        {
            ModalParameters parameters = new ModalParameters();
            parameters.Add(nameof(ModalInfo.Msj), msj);
            parameters.Add(nameof(ModalInfo.CssClass), cssClass);

            return parameters;
        }
    }
}
