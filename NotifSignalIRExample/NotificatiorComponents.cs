using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NotifSignalIRExample
{
    public class NotificatiorComponents
    {
        //Aqui agregaremos las funciones de laas notificaciones del registro  y agregamos el sql dependency
        public void RegisterNotification(DateTime currentTime)
        {
            string conStr = ConfigurationManager.ConnectionStrings["sqlConString"].ConnectionString;
            string sqlCommand = @"SELECT [ContactID],[ContactName],[ContactNo] from [dbo].[Contacts] where [AddedOn] > @AddedOn";
            //tu puedes notificar aca, y tiene que agregar el nombre de la tabla es [dbo].[Contacts], esto es obligatorio cuando usas SQL DEPENDECY
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);
                cmd.Parameters.AddWithValue("@AddedOn", currentTime);
                if(con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += SqlDep_OnChange;
                // Debemos tener que ejecutar el comando  aca
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    
                }

            }
        }

        private void SqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= SqlDep_OnChange;
                //aca se enviara la n otificacion del mensaje para el cliente
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<notificationHub>();
                notificationHub.Clients.All.notify("added");

                //registro de notificacion
                RegisterNotification(DateTime.Now);

            }
        }
        public List<Contact> GetContacts(DateTime afterDate)
        {
            using (MyPushNotifEntities dc = new MyPushNotifEntities())
            {
                return dc.Contacts.Where(a => a.AddedOn > afterDate).OrderByDescending(a => a.AddedOn).ToList();

            }
        }
    }
}