using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Battlefield : MonoBehaviour
{
    public Holding holding;
    public Vector2Int size;

    public Tile[,] battlefield;
    public Army armyA;
    public Army armyB;
    public List<Unit> offenseUnits;

    //put in 2 armies, get out one victor
    [Button]
    public void InitializeBattlefield()
    {
        battlefield = CreateBattlefield(armyA, armyB);
    }

    public Tile[,] CreateBattlefield(Army offense, Army defense)
    {
        Tile[,] battlefield = new Tile[size.x, size.y];

        #region Offense
        var offenseStance = Resources.Load<TacticsManager>("TacticsManager").armyStances[(int)offense.stance];

        offenseUnits.Clear();
        for (int i = 0; i < armyA.units.Count; i++)
        {
            offenseUnits.Add(armyA.units[i]);
        }

        int offenseTotal = offenseUnits.Count;

        for (int x = 0; x < size.x; x++)
        {
            //offense
            if (x < (size.x / 2) && x >= ((size.x / 2) - offenseStance.lines.Length))
            {
                //loop thru each unit type***
                //for each one fill up amount of squares based on how many units the army has***
                //total num of units placed in column
                //take half of it and subtract it from column length
                //length - 1?
                //add amount to index based on how far from center it is

                int newSize = offenseTotal <= size.y ? offenseTotal : size.y;

                for (int y = 0; y < newSize; y++)
                {
                    for (int j = 0; j < offenseStance.lines[(size.x / 2) - x - 1].units.Length; j++)
                    {
                        var unitReference = offenseStance.lines[(size.x / 2) - x - 1].units[j];
                        int offenseUnitIndex = 0;
                        print(y);
                        print(Mathf.RoundToInt(unitReference.low * newSize));
                        print(Mathf.RoundToInt(unitReference.high * newSize));

                        if (y >= Mathf.RoundToInt(unitReference.low * newSize) && y < Mathf.RoundToInt(unitReference.high * newSize) && OffenseContains(unitReference.unitType))
                        {
                            battlefield[x, y] = new Tile(offenseUnits[offenseUnitIndex]);
                            offenseUnits.Remove(offenseUnits[offenseUnitIndex]);
                            break;
                        }

                        bool OffenseContains(UnitType unitType)
                        {
                            for (int u = 0; u < offenseUnits.Count; u++)
                            {
                                if (offenseUnits[u].unitType == unitType)
                                {
                                    offenseUnitIndex = u;
                                    return true;
                                }
                            }
                            return false;
                        }
                    }
                }

                for (int y = 0; y < newSize; y++)
                {
                    if (battlefield[x, y] == null)
                    {
                        for (int i = 0; i < offenseUnits.Count; i++)
                        {
                            battlefield[x, y] = new Tile(offenseUnits[i]);
                            offenseUnits.Remove(offenseUnits[i]);
                            break;
                        }
                    }
                }

                for (int y = 0; y < size.y; y++)
                {
                    if (battlefield[x, y] != null)
                    {
                        print(x + " " + battlefield[x, y].occupyingUnit.unitType);
                    }
                    else
                    {
                        print(null);
                    }
                }

                //List<int> roundedInts = new List<int>();
                //int total = 0;
                //int difference = 0;

                //for (int i = 0; i < offenseStance.lines[(size.x / 2) - x - 1].units.Length; i++)
                //{
                //    var unitReference = offenseStance.lines[(size.x / 2) - x - 1].units[i];

                //    int percentage = Mathf.FloorToInt(unitReference.high * newSize - unitReference.low * newSize);
                //    roundedInts.Add(percentage);
                //}

                //for (int i = 0; i < roundedInts.Count; i++)
                //{
                //    total += roundedInts[i];
                //}

                //difference = newSize - total;

                //for (int i = 0; i < roundedInts.Count; i++)
                //{
                //    if (difference == 0)
                //    {
                //        break;
                //    }

                //    if (roundedInts[i] % 2 != 0)
                //    {
                //        difference--;
                //        roundedInts[i]++;
                //    }
                //}

                //for (int i = 0; i < roundedInts.Count; i++)
                //{
                //    print(roundedInts[i]);
                //}
                //print(difference);

                //if largest num and odd? or 0

                //for (int y = 0; y < newSize; y++)
                //{
                //    for (int j = 0; j < offenseStance.lines[(size.x / 2) - x - 1].units.Length; j++)
                //    {
                //        var unitReference = offenseStance.lines[(size.x / 2) - x - 1].units[j];
                //        int offenseUnitIndex = 0;

                //        if (y >= (int)(unitReference.low * newSize) && y < (int)(unitReference.high * newSize) && OffenseContains(unitReference.unitType))
                //        {
                //            battlefield[x, y] = new Tile(offenseUnits[offenseUnitIndex]);
                //            offenseUnits.Remove(offenseUnits[offenseUnitIndex]);
                //            break;
                //        }

                //        bool OffenseContains(UnitType unitType)
                //        {
                //            for (int u = 0; u < offenseUnits.Count; u++)
                //            {
                //                if (offenseUnits[u].unitType == unitType)
                //                {
                //                    offenseUnitIndex = u;
                //                    return true;
                //                }
                //            }
                //            return false;
                //        }
                //    }
                //}
                //for (int y = 0; y < size.y; y++)
                //{
                //    if (battlefield[x, y] != null)
                //    {
                //        print(x + " " + battlefield[x, y].occupyingUnit.unitType);
                //    }
                //    else
                //    {
                //        print(null);
                //    }
                //}

                //int newSize = offenseTotal <= size.y ? offenseTotal : size.y;


            }
        }

        //for (int x = 0; x < size.x; x++)
        //{
        //    for (int y = 0; y < size.y; y++)
        //    {
        //        if (battlefield[x, y] != null)
        //        {
        //            print(battlefield[x, y].occupyingUnit);
        //        }
        //        else
        //        {
        //            print(null);
        //        }
        //    }
        //}
        #endregion

        return battlefield;
    }

    public class Tile
    {
        public Unit occupyingUnit;

        public Tile(Unit occupyingUnit)
        {
            this.occupyingUnit = occupyingUnit;
        }
    }
}
