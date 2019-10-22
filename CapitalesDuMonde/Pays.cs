using System;
using System.Collections.Generic;
using System.Text;

namespace CapitalesDuMonde
{
    class Pays
    {

    }
}
// REMARQUE : Le code généré peut nécessiter au moins .NET Framework 4.5 ou .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class Monde
{

    private MondeContinent[] continentField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("continent")]
    public MondeContinent[] Continent
    {
        get
        {
            return this.continentField;
        }
        set
        {
            this.continentField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class MondeContinent
{

    private string nomContienent;

    private MondeContinentPays[] listePays;

    /// <remarks/>
    public string Nom
    {
        get
        {
            return this.nomContienent;
        }
        set
        {
            this.nomContienent = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("pays", IsNullable = false)]
    public MondeContinentPays[] Liste
    {
        get
        {
            return this.listePays;
        }
        set
        {
            this.listePays = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class MondeContinentPays
{

    private string nom;

    private string capitale;

    private string monnaie;

    /// <remarks/>
    public string Nom
    {
        get
        {
            return this.nom;
        }
        set
        {
            this.nom = value;
        }
    }

    /// <remarks/>
    public string Capitale
    {
        get
        {
            return this.capitale;
        }
        set
        {
            this.capitale = value;
        }
    }

    /// <remarks/>
    public string Monnaie
    {
        get
        {
            return this.monnaie;
        }
        set
        {
            this.monnaie = value;
        }
    }
}


