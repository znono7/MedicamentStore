using MedicamentStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public static class ProduitsPharmaceutiquesTypeExtensions
    {
        public static string ToProduitsPharmaceutiques(this ProduitsPharmaceutiquesType type)
        {
            switch (type) 
            {
                case ProduitsPharmaceutiquesType.None:
                    return "Tous les Produits pharmaceutiques";
                case ProduitsPharmaceutiquesType.Medicaments:
                    return "Médicaments";
                case ProduitsPharmaceutiquesType.MedicamentsPourPrevention:
                    return "Médicaments pour Prévention";
                case ProduitsPharmaceutiquesType.MedicamentsPourDAT:
                    return "Médicaments pour DAT";
                case ProduitsPharmaceutiquesType.MedicamentsPourGenevologie:
                    return "Médicaments pour Généalogie";
                case ProduitsPharmaceutiquesType.ArticleDePansements:
                    return "Articles de Pansements";
                case ProduitsPharmaceutiquesType.AntiSeptiques:
                    return "Antiseptiques";
                case ProduitsPharmaceutiquesType.Consommables:
                    return "Consommables";
                case ProduitsPharmaceutiquesType.ProduitsDentaires:
                    return "Produits Dentaires";
                case ProduitsPharmaceutiquesType.FilmsRadiologiques:
                    return "Films Radiologiques";
                case ProduitsPharmaceutiquesType.SolutesMassifs:
                    return "Solutés Massifs";
                case ProduitsPharmaceutiquesType.ContaceptifsOraux:
                    return "Contraceptifs Oraux";
                case ProduitsPharmaceutiquesType.Reactifs:
                    return "Réactifs";
                case ProduitsPharmaceutiquesType.MaterielsDeLaboratoires:
                    return "Matériels de Laboratoires";
                case ProduitsPharmaceutiquesType.Psychotropes:
                    return "Psychotropes";
                case ProduitsPharmaceutiquesType.VaccinsEtSerums:
                    return "Vaccins et Sérums";
                case ProduitsPharmaceutiquesType.Appareils:
                    return "Appareils";
                case ProduitsPharmaceutiquesType.ProduitsPourDiagnostic:
                    return "Produits pour Diagnostic";
                case ProduitsPharmaceutiquesType.MoyensDeProtection:
                    return "Moyens de Protection";
                case ProduitsPharmaceutiquesType.Sondes:
                    return "Sondes";
                case ProduitsPharmaceutiquesType.Masques:
                    return "Masques";
                case ProduitsPharmaceutiquesType.Papier:
                    return "Papier";
                case ProduitsPharmaceutiquesType.Gel:
                    return "Gel";
                default:
                    return null;
            }
        }

        public static ProduitsPharmaceutiquesType ToProduiyPharmaType(this string type)
        {
            if (type == "Tous les Produits pharmaceutiques")
                return ProduitsPharmaceutiquesType.None;
            if (type == "Médicaments")
                return ProduitsPharmaceutiquesType.Medicaments; 
            if (type == "Médicaments pour Prévention")
                return ProduitsPharmaceutiquesType.MedicamentsPourPrevention;
            if (type == "Médicaments pour DAT")
                return ProduitsPharmaceutiquesType.MedicamentsPourDAT;
            if (type == "Médicaments pour Généalogie")
                return ProduitsPharmaceutiquesType.MedicamentsPourGenevologie;
            if (type == "Articles de Pansements")
                return ProduitsPharmaceutiquesType.ArticleDePansements;
            if (type == "Antiseptiques")
                return ProduitsPharmaceutiquesType.AntiSeptiques;
            if (type == "Consommables")
                return ProduitsPharmaceutiquesType.Consommables;
            if (type == "Produits Dentaires")
                return ProduitsPharmaceutiquesType.ProduitsDentaires;
            if (type == "Films Radiologiques")
                return ProduitsPharmaceutiquesType.FilmsRadiologiques;
            if (type == "Solutés Massifs")
                return ProduitsPharmaceutiquesType.SolutesMassifs;
            if (type == "Contraceptifs Oraux")
                return ProduitsPharmaceutiquesType.ContaceptifsOraux;
            if (type == "Réactifs")
                return ProduitsPharmaceutiquesType.Reactifs;
            if (type == "Matériels de Laboratoires")
                return ProduitsPharmaceutiquesType.MaterielsDeLaboratoires;
            if (type == "Psychotropes")
                return ProduitsPharmaceutiquesType.Psychotropes;
            if (type == "Vaccins et Sérums")
                return ProduitsPharmaceutiquesType.VaccinsEtSerums;
            if (type == "Appareils")
                return ProduitsPharmaceutiquesType.Appareils;
            if (type == "Produits pour Diagnostic")
                return ProduitsPharmaceutiquesType.ProduitsPourDiagnostic;
            if (type == "Moyens de Protection")
                return ProduitsPharmaceutiquesType.MoyensDeProtection;
            if (type == "Sondes")
                return ProduitsPharmaceutiquesType.Sondes;
            if (type == "Masques")
                return ProduitsPharmaceutiquesType.Masques;
            if (type == "Papier")
                return ProduitsPharmaceutiquesType.Papier;
            if (type == "Gel")
                return ProduitsPharmaceutiquesType.Gel;

            Debugger.Break();
            return default(ProduitsPharmaceutiquesType);
        }

    }
}
//List<string> produitsPharmaceutiquesList = new List<string>();

//foreach (ProduitsPharmaceutiquesType type in Enum.GetValues(typeof(ProduitsPharmaceutiquesType)))
//{
//    string convertedValue = type.ToProduitsPharmaceutiques();
//    if (convertedValue != null)
//    {
//        produitsPharmaceutiquesList.Add(convertedValue);
//    }
//}
