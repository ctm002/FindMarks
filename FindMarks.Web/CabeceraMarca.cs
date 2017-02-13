using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrackMarks.Web
{
    public class CabeceraMarca
    {
        public CabeceraMarca()
        {
            Hash = string.Empty;
            Marcas = new List<DetalleMarca>();
        }

        public string Hash { get; set; }

        public List<DetalleMarca> Marcas;
    }
}