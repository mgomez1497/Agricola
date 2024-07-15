using System;
using System.Collections.Generic;

namespace Api_agricola.Models;

public partial class Lote
{
    public int Id { get; set; }

    public int? IdFinca { get; set; }

    public string Nombre { get; set; } = null;

    public int Arboles { get; set; }

    public string Etapa { get; set; } = null!;


}
