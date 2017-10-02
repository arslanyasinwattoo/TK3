package com.example.arslanyasinwattoo.inventorysystem;

/**
 * Created by Arslanyasinwattoo on 6/25/2016.
 */
public class InventoryClass {

    private String itemId;
    private String itemName;
    private int quantity;

    public InventoryClass(String itemId, String itemName, int quantity) {
        this.itemId = itemId;
        this.itemName = itemName;
        this.quantity = quantity;
    }

    public String getItemId() {
        return itemId;
    }

    public void setItemId(String itemId) {
        this.itemId = itemId;
    }

    public String getItemName() {
        return itemName;
    }

    public void setItemName(String itemName) {
        this.itemName = itemName;
    }

    public int getQuantity() {
        return quantity;
    }

    public void setQuantity(int quantity) {
        this.quantity = quantity;
    }
}