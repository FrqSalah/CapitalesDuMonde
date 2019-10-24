using System;
using System.Collections.Generic;
using System.Text;

namespace CapitalesDuMonde
{
}
public class Rootobject
{
    public Monde monde { get; set; }
}

public class Monde
{
    public Continent[] continent { get; set; }
}

public class Continent
{
    public string nom { get; set; }
    public Pays[] pays { get; set; }
}

public class Pays
{
    public string nom { get; set; }
    public string capitale { get; set; }
    public string monnaie { get; set; }
}
