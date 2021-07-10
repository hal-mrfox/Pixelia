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
        
        for (int x = 0; x < offenseStance.lines.Length; x++)
        {
            int shavedTotal = offenseUnits.Count;
            for (int i = 0; i < offenseUnits.Count; i++)
            {
                if (!LineContains(offenseUnits[i].unitType))
                {
                    shavedTotal--;
                }
            }

            bool LineContains(UnitType unitType)
            {
                bool found = false;
                for (int i = 0; i < offenseStance.lines[x].units.Length; i++)
                {
                    if (unitType == offenseStance.lines[x].units[i].unitType)
                    {
                        found = true;
                        break;
                    }
                    else
                    {
                        found = false;
                    }
                }
                return found;
            }

            int newSize = shavedTotal <= size.y ? shavedTotal : size.y; //shaved off units that cant fit in
            List<Vector2Int> unitCaps = new List<Vector2Int>(); //used to lower minimum of high-low ranges
            List<UnitType> allowedUnits = new List<UnitType>();

            for (int i = 0; i < offenseStance.lines[x].units.Length; i++)
            {
                var unitReference = offenseStance.lines[x].units[i];
                unitCaps.Add(new Vector2Int(Mathf.RoundToInt(unitReference.low * newSize), Mathf.RoundToInt(unitReference.high * newSize)));
            }

            for (int y = 0; y < newSize; y++)
            {
                for (int j = 0; j < offenseStance.lines[x].units.Length; j++)
                {
                    if (!allowedUnits.Contains(offenseStance.lines[x].units[j].unitType))
                    {
                        allowedUnits.Add(offenseStance.lines[x].units[j].unitType);
                    }
                    var unitReference = offenseStance.lines[x].units[j];
                    int offenseUnitIndex = 0;
                    print(y);
                    print(unitCaps[j].x);
                    print(unitCaps[j].y);
                    print(unitReference.unitType);
                    print(y >= unitCaps[j].x && y < unitCaps[j].y && OffenseContains(unitReference.unitType));
                    //MAKE CENTERED
                    //ADD UNUSED UNITS TO BACK LINES

                    if (y >= unitCaps[j].x && y < unitCaps[j].y && OffenseContains(unitReference.unitType))
                    {
                        battlefield[((size.x / 2) - 1) - x, y] = new Tile(offenseUnits[offenseUnitIndex]);
                        offenseUnits.Remove(offenseUnits[offenseUnitIndex]);
                        break;
                    }
                    else if (y >= unitCaps[j].x && y < unitCaps[j].y && !OffenseContains(unitReference.unitType) && j < offenseStance.lines[x].units.Length - 1)
                    {
                        unitCaps[j] = new Vector2Int(unitCaps[j].x, y);
                        unitCaps[j + 1] = new Vector2Int(y, unitCaps[j + 1].y);
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
        }
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
