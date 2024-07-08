using Microsoft.AspNetCore.Mvc;

namespace ContaFacil.Logica
{
    public class NotificacionClass : Controller
    {
        string pos = "";
        public enum NotificacionTipo
        {
            Success,
            Error,
            Warning
        }
        public enum NotificationPosition
        {
            Top,
            TopStart,
            TopEnd,
            Center,
            CenterStart,
            CenterEnd,
            Bottom,
            BottomStart,
            BottomEnd
        }
        public void BasicNotificacion(string mensaje, NotificacionTipo tipo, string titulo = "")
        {
            TempData["notificacion"] = $"Swal.fire('{titulo}','{mensaje}', '{tipo.ToString().ToLower()}')";

        }
        public void CustomNotification(string msj, NotificacionTipo type, NotificationPosition position, string title = "", bool showConfirmButton = false, int timer = 2000, bool toast = true)
        {
            SetPosition(position.ToString());

            TempData["notification"] = "Swal.fire({customClass:{confirmButton:'btn btn-primary',cancelButton:'btn btn-danger'},position:'" + pos + "',type:'" + type.ToString().ToLower() +
                "',title:'" + title + "',text: '" + msj + "',showConfirmButton: " + showConfirmButton.ToString().ToLower() + ",confirmButtonColor: '#4F0DA2',toast: "
                + toast.ToString().ToLower() + ",timer: " + timer + "}); ";
        }
        #region Methods

        private void SetPosition(string position)
        {
            if (position == "Top") pos = "top";
            if (position == "TopStart") pos = "top-start";
            if (position == "TopEnd") pos = "top-end";
            if (position == "Center") pos = "center";
            if (position == "CenterStart") pos = "center-start";
            if (position == "CenterEnd") pos = "center-end";
            if (position == "Bottom") pos = "bottom";
            if (position == "BottomStart") pos = "bottom-start";
            if (position == "BottomEnd") pos = "bottom-end";
        }

        #endregion

        public void Notificacion(string mensaje, NotificacionTipo tipo)
        {
            var script = "Swal.fire({backdrop: false,position: 'top-end',icon: '" + tipo.ToString().ToLower() + "', title: '" + mensaje + "', showConfirmButton: false, timer: 3000,customClass: 'custom-alert',allowOutsideClick: false})";
            TempData["notificacion"] = script;
        }

    }
}
