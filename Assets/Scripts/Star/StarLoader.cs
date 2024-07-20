using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class StarLoader
{
    public static List<Star> stars;
    public static List<string> constellations;
    public static List<List<(int, int)>> constellationships;
    public static List<int> constellationStars;
    public static List<(float, float)> constellationPositions;
    public static Dictionary<string, string> IAUtoProperName;
    public static Dictionary<string, string> IAUtoGenitive;
    public static void LoadData(int starFieldScale)
    {
        LoadStars();
        LoadConstellationStars();
        LoadConstellationNames();
        LoadConstellations(starFieldScale);
        CalculateConstellationPosition();
    }
    public static void LoadStars()
    {
        string splitStr = ",";
        stars = new List<Star>();

        // Read the csv file
        const string filename = "fixed_hyg_dataset";
        TextAsset textAsset = Resources.Load(filename) as TextAsset;
        StringReader reader = new StringReader(textAsset.text);
        while(reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            string[] items = line.Split(splitStr.ToCharArray(), System.StringSplitOptions.None);
            if (items[0] == "\"id\"" || items[0] == "0") continue;
            Star star = new Star(items);
            stars.Add(star);
        }
    }

    public static void LoadConstellationStars()
    {
        constellationStars = new List<int>();

        const string filename = "constellation_stars";
        TextAsset textAsset = Resources.Load<TextAsset>(filename) as TextAsset;
        StringReader reader = new StringReader(textAsset.text);
        while(reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            int item = int.Parse(line);
            constellationStars.Add(item);
        }
    }

    public static void LoadConstellations(float starFieldScale)
    {
        constellations = new();
        constellationships = new();
        const string filename = "constellationship";
        TextAsset textAsset = Resources.Load(filename) as TextAsset;
        StringReader reader = new StringReader(textAsset.text);

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            string[] items = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            constellations.Add(IAUtoProperName[items[0]]);
            int n = int.Parse(items[1]);
            List<(int, int)> constellationship = new();

            for (int i = 1; i <= n; i++)
            {
                int a = int.Parse(items[2 * i]);
                int b = int.Parse(items[2 * i + 1]);

                constellationship.Add((a, b));
            }
            constellationships.Add(constellationship);
        }
    }

    public static void LoadConstellationNames()
    {
        IAUtoProperName = new();
        IAUtoGenitive = new();
        //const string constellationNameFile = "constellationname";
        //TextAsset constellationNameAsset = Resources.Load(constellationNameFile) as TextAsset;
        //StringReader constellationReader = new StringReader(constellationNameAsset.text);

        //while (constellationReader.Peek() != -1)
        //{
        //    string line = constellationReader.ReadLine();
        //    string[] items = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        //    if (items[0] == "Constellation") continue;
        //    string name = items[0].Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
        //    IAUtoProperName.Add(items[1], name);
        //}

        List<Dictionary<string, object>> data = CSVReader.Read("constellationname");

        for (var i = 0; i < data.Count; i++)
        {
            string name = data[i]["Constellation"] as string;
            name = name.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
            IAUtoProperName.Add(data[i]["IAU"] as string, name);

            string gen = data[i]["Genitive"] as string;
            gen = gen.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
            IAUtoGenitive.Add(data[i]["IAU"] as string, gen);
        }
    }

    public static void CalculateConstellationPosition()
    {
        constellationPositions = new();
        List<int> numberOfStars = new();
        List<float> sinRA = new();
        List<float> cosRA = new();
  
        for(int i=0; i<constellations.Count; i++)
        {
            constellationPositions.Add((0,0));
            numberOfStars.Add(0);
            sinRA.Add(0);
            cosRA.Add(0);
        }
        for(int i=0; i<constellationStars.Count; i++) 
        {
            Star star = stars[constellationStars[i] - 1];
            int consId = constellations.IndexOf(IAUtoProperName[star.con]);
            if (consId != -1)
            {
                sinRA[consId] += Mathf.Sin((float)star.ra * 15 * Mathf.Deg2Rad);
                cosRA[consId] += Mathf.Cos((float)star.ra * 15 * Mathf.Deg2Rad);
                float dec = constellationPositions[consId].Item2 + (float)star.dec;
                constellationPositions[consId] = (0,dec);
                numberOfStars[consId]++;
            }
        }
        for(int i=0; i<constellationPositions.Count; i++)
        {
            float sin = sinRA[i] / numberOfStars[i];
            float cos = cosRA[i] / numberOfStars[i];
            float ra = Mathf.Atan2(sin, cos) * Mathf.Rad2Deg / 15;
            float dec = constellationPositions[i].Item2 / numberOfStars[i];
            constellationPositions[i] = (ra, dec);
        }
    }

}
