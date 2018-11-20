using Photon.Pun;
using UnityEngine;
using System;

public abstract class EquipmentSlot : MonoBehaviourPunCallbacks
{

    protected Item currentEquipment;
    protected GameObject equipedItem;

    public event Action OnEquip = delegate { };

    public virtual void Initialise(bool local)
    {
        if (!local)
        {
            enabled = false;
        }
    }

    // Equips the item 'toEquip' at 'spawnpoint' and returns two PhotonView ID's to use for parenting the item via an RPC.
    protected int[] Equip(Item toEquip, Transform spawnpoint)
    {
        if (!photonView.IsMine)
        {
            return null;
        }

        if (equipedItem)
        {
            PhotonNetwork.Destroy(equipedItem);
        }

        currentEquipment = toEquip;

        if (toEquip != null)
        {
            equipedItem = PhotonNetwork.Instantiate(currentEquipment.itemPrefab.name, spawnpoint.position, spawnpoint.rotation);
            OnEquip();

            int equipmentID = equipedItem.GetComponent<PhotonView>().ViewID;
            int spawnpointID = spawnpoint.GetComponent<PhotonView>().ViewID;

            return new int[] { equipmentID, spawnpointID };
        }

        return null;
    }

    protected abstract void ParentEquipment(int equipmentID, int parentID);

    protected int[] GetEquipedItemIDs(Transform spawnpoint)
    {
        if (currentEquipment == null)
        {
            return null;
        }

        int equipmentID = equipedItem.GetComponent<PhotonView>().ViewID;
        int spawnpointID = spawnpoint.GetComponent<PhotonView>().ViewID;

        return new int[] { equipmentID, spawnpointID };
    }
}
