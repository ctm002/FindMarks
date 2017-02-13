using System.Collections.Generic;

namespace TrackMarks.Web
{
    public class DetalleSolicitud
    {
        public  List<string> Anotaciones { get; set; }
        public  string Audio { get; set; }
        public  List<Clase> Clases { get; set; }
        public  string Denominacion { get; set; }
        public  string Estado { get; set; }
        public  string EstadoDescripcion { get; set; }
        public  string EstadoIPais { get; set; }
        public  string Etiqueta { get; set; }
        public  string EtiquetaDescripcion { get; set; }
        public  string Fallo { get; set; }
        public  string FechaPresentacion { get; set; }
        public  string FechaPublicacion { get; set; }
        public  string FechaRegistro { get; set; }
        public  string File { get; set; }
        public  List<Instancia> Instancias { get; set; }
        public  string NumeroPoder { get; set; }
        public  string NumeroRegistro { get; set; }
        public int NumeroRegistroAplicaFrase { get; set; }
        public  int NumeroRegistroRenovar { get; set; }
        public  string NumeroSolicitud { get; set; }
        public  int NumeroSolicitudAplicaFrase { get; set; }
        public  string NumeroSolicitudRenovada { get; set; }
        public int NumeroSolicitudRenovar { get; set; }
        public  int NumeroSolicitudRenueva { get; set; }
        public  List<Prioridad> Prioridad { get; set; }
        public  string Proteccion { get; set; }
        public  List<string> Regiones { get; set; }
        public  string RegistroBase { get; set; }
        public  string Representante { get; set; }
        public List<Persona> Representantes { get; set; }
        public  string TipoCategoria { get; set; }
        public  string TipoCategoriaDescripcion { get; set; }
        public  int TipoCobertura { get; set; }
        public  int TipoCoberturaAplicaFrase { get; set; }
        public  string TipoCoberturaDescripcion { get; set; }
        public string TipoMarca { get; set; }
        public  string TipoMarcaDescripcion { get; set; }
        public  List<Persona> Titulares { get; set; }
        public string Traduccion { get; set; }
    }
}