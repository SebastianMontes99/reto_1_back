using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace wcf_reservas
{
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        public void HandleOptions() {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "https://localhost:44391");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
        }

        public bool AuthenticateUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-LN8NCIO;Initial Catalog=sistema_reserva_libros;Integrated Security=true;"))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE varEmail = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", username);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //esto debería hacerse de manera segura, como comparar hashes
                            if (reader["varPassword"].ToString() == password)
                            {
                                bool isActive = Convert.ToBoolean(reader["bolIsActive"]);
                                return isActive;
                            }
                        }

                        return false;
                    }
                }
            }
        }

        public bool ReservarLibro(string idLibro, string idUsuario)
        {
            ReservaData reservaData = OperationContext.Current.RequestContext.RequestMessage.GetBody<ReservaData>();

            using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-LN8NCIO;Initial Catalog=sistema_reserva_libros;Integrated Security=true;"))
            {
                connection.Open();

                using (SqlCommand checkReservationCommand = new SqlCommand("SELECT TOP 1 1 FROM Reservations WHERE idBook = @IdLibro AND bolIsActive = 1", connection))
                {
                    checkReservationCommand.Parameters.AddWithValue("@IdLibro", reservaData.IdLibro);

                    object result = checkReservationCommand.ExecuteScalar();

                    if (result != null)
                    {
                        return false;
                    }
                }

                using (SqlCommand reserveBookCommand = new SqlCommand("UPDATE Books SET bolIsActive = 1 WHERE idBook = @IdLibro", connection))
                {
                    reserveBookCommand.Parameters.AddWithValue("@IdLibro", reservaData.IdLibro);

                    int rowsAffected = reserveBookCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        using (SqlCommand createReservationCommand = new SqlCommand("INSERT INTO Reservations (idUser, idBook, dmeDateReservation, instStatus, dmeDateCreate, dmeDateUpdate, bolIsActive) VALUES (@IdUsuario, @IdLibro, GETDATE(), 1, GETDATE(), GETDATE(), 1)", connection))
                        {
                            createReservationCommand.Parameters.AddWithValue("@IdUsuario", reservaData.IdUsuario);
                            createReservationCommand.Parameters.AddWithValue("@IdLibro", reservaData.IdLibro);

                            int reservationRowsAffected = createReservationCommand.ExecuteNonQuery();

                            return reservationRowsAffected > 0;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }



        public List<Libro> ObtenerLibros()
        {
            List<Libro> libros = new List<Libro>();

            using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-LN8NCIO;Initial Catalog=sistema_reserva_libros;Integrated Security=true;"))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Books", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Libro libro = new Libro
                            {
                                IdBook = Convert.ToInt32(reader["idBook"]),
                                Titulo = Convert.ToString(reader["varTitle"]),
                                Codigo = Convert.ToString(reader["varCode"]),
                                InstStatus = Convert.ToInt32(reader["instStatus"]),
                                DmeDateCreate = Convert.ToDateTime(reader["dmeDateCreate"]),
                                DmeDateUpdate = Convert.ToDateTime(reader["dmeDateUpdate"]),
                                BolIsActiver = Convert.ToBoolean(reader["bolIsActive"])
                            };

                            libros.Add(libro);
                        }
                    }
                }
            }

            return libros;
        }

        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
