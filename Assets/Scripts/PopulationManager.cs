using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PopTier { Slave, Resident, Citizen, Elite };

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager instance;

    public PopTierDetails[] popTierDetails;

    [System.Serializable]
    public class PopTierDetails
    {
        public PopTier popTier;
        public Color popColor;
        public Resource[] needs;
    }

    public void Awake()
    {
        instance = this;
    }
}
