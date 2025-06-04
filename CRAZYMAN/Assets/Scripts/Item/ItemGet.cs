using UnityEngine;
using Photon.Pun;

public class ItemGet : MonoBehaviourPunCallbacks
{
    public Item item; // ȹ���� ������
    private PhotonView itemPhotonView;
    private Inventory nearbyPlayerInventory = null;
    private bool isPlayerNearby = false; // �÷��̾ ������ ��ó�� �ִ��� Ȯ���ϴ� ����

    private void Awake()
    {
        itemPhotonView = GetComponent<PhotonView>();
        if (itemPhotonView == null)
        {
            Debug.LogError("ItemGet: ������ ������Ʈ�� PhotonView ������Ʈ�� �����ϴ�! ��Ʈ��ũ ���� �Ұ�!");
            enabled = false; // PhotonView ������ ��ũ��Ʈ �ߴ�
            return;
        }
        // ItemGet ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ���� Item ������Ʈ ��������
        item = GetComponent<Item>();
        if (item == null)
        {
            Debug.LogError("ItemGet: Item ������Ʈ�� �����ϴ�! ȹ�� �Ұ�!");
            enabled = false; // Item ������Ʈ ������ ��ũ��Ʈ �ߴ�
            return;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �浹�� ������Ʈ���� PhotonView ������Ʈ�� ������
            PhotonView playerPhotonView = collision.GetComponent<PhotonView>();

            if (playerPhotonView.IsMine)
            {
                Debug.Log($"ItemGet: ���� �÷��̾�({playerPhotonView.ViewID})�� �����ۿ� ����! ({gameObject.name}). F Ű �Է� ���.");

                nearbyPlayerInventory = collision.GetComponent<Inventory>(); // �浹�� �÷��̾�� Inventory ��������
                
                // �÷��̾ ������ ��ó�� �ִٴ� ���� ���� ����
                isPlayerNearby = true;
                
                if (nearbyPlayerInventory == null)
                {
                    Debug.LogError("ItemGet: ���� �÷��̾�� Inventory ������Ʈ�� �����ϴ�! ȹ�� �Ұ�!");
                }

            }
            else
            {
                Debug.Log($"ItemGet: �ٸ� �÷��̾�({playerPhotonView.ViewID})�� �����ۿ� ����.");
            }
        }

        /*if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = true;

            playerInventory = collision.GetComponent<Inventory>();

            if (playerInventory == null)
            {
                Debug.LogWarning("�÷��̾ Invetory ��ũ��Ʈ�� ����");
            }
            Debug.Log("������ ���� ���� F Ű ������ ����");
        }*/
    }

    [PunRPC]
    void RemoveItemRPC(PhotonMessageInfo info)
    {
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("�־���");
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F Ű �Է�!");

            ItemDataForInventory itemData = new ItemDataForInventory(item);

            if (nearbyPlayerInventory.AddItem(itemData)) // ������ �߰� ���� ��
            {
                Debug.Log("�κ��丮�� ������ �߰� ����!");

                itemPhotonView.RPC("RemoveItemRPC", RpcTarget.All); // ��� Ŭ���̾�Ʈ���� ������ ����
            }
            else
            {
                Debug.LogWarning("�κ��丮�� ������ �߰� ����!"); // �κ��丮 ���� á�� �� ��
            }

            isPlayerNearby = false; // ���� �� �ʱ�ȭ
            nearbyPlayerInventory = null;
        }
    }
}