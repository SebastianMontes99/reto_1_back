using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace wcf_reservas
{
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        bool AuthenticateUser(string username, string password);

        [OperationContract]
        List<Libro> ObtenerLibros();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "ReservarLibro", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool ReservarLibro(string idLibro, string idUsuario);

        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
        void HandleOptions();

    }


    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

    [DataContract]
    public class Libro
    {
        [DataMember]
        public int IdBook { get; set; }

        [DataMember]
        public string Titulo { get; set; }

        [DataMember]
        public string Codigo { get; set; }

        [DataMember]
        public int InstStatus { get; set; }

        [DataMember]
        public DateTime DmeDateCreate { get; set; }

        [DataMember]
        public DateTime DmeDateUpdate { get; set; }

        [DataMember]
        public bool BolIsActiver { get; set; }
    }


    [DataContract]
    public class ReservaData
    {
        [DataMember]
        public string IdLibro { get; set; }

        [DataMember]
        public string IdUsuario { get; set; }
    }
}
