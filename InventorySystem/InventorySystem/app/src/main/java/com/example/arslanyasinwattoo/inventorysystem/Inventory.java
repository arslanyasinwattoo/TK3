package com.example.arslanyasinwattoo.inventorysystem;

import android.app.Activity;
import android.graphics.Color;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.Gravity;
import android.widget.ArrayAdapter;
import android.widget.GridView;
import android.widget.ListView;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;
import android.widget.Toast;

import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;

/**
 * Created by Arslanyasinwattoo on 6/25/2016.
 */
public class Inventory extends AppCompatActivity {
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.inventory);
        new HttpAsyncTask().execute("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetInventory");
    }
    public static String GET(String url){
        InputStream inputStream = null;
        String result = "";
        try {

            // create HttpClient
            HttpClient httpclient = new DefaultHttpClient();

            // make GET request to the given URL
            HttpResponse httpResponse = httpclient.execute(new HttpGet(url));

            // receive response as inputStream
            inputStream = httpResponse.getEntity().getContent();

            // convert inputstream to string
            if(inputStream != null)
                result = convertInputStreamToString(inputStream);
            else
                result = "Did not work!";

        } catch (Exception e) {
            Log.d("InputStream", e.getLocalizedMessage());
        }

        return result;
    }

    private static String convertInputStreamToString(InputStream inputStream) throws IOException {
        BufferedReader bufferedReader = new BufferedReader( new InputStreamReader(inputStream));
        String line = "";
        String result = "";
        while((line = bufferedReader.readLine()) != null)
            result += line;

        inputStream.close();
        return result;

    }

    public boolean isConnected(){
        ConnectivityManager connMgr = (ConnectivityManager) getSystemService(Activity.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connMgr.getActiveNetworkInfo();
        if (networkInfo != null && networkInfo.isConnected())
            return true;
        else
            return false;
    }
    private class HttpAsyncTask extends AsyncTask<String, Void, String> {
        @Override
        protected String doInBackground(String... urls) {

            return GET(urls[0]);
        }
        // onPostExecute displays the results of the AsyncTask.
        @Override
        protected void onPostExecute(String result) {
            Log.d("Testing", result);
            try {
                Toast.makeText(getBaseContext(),"Recieve", Toast.LENGTH_LONG).show();
                JSONArray items = new JSONArray(result);
                ArrayList<String> sl= new ArrayList<>();
                TableLayout stk = (TableLayout) findViewById(R.id.table2);
                TableRow tbrow0 = new TableRow(Inventory.this);
                TextView tv0 = new TextView(Inventory.this);
                tv0.setText(" Id ");
                tv0.setGravity(Gravity.CENTER);
                tv0.setTextColor(Color.WHITE);
                tbrow0.addView(tv0);
                TextView tv1 = new TextView(Inventory.this);
                tv1.setText(" Item ");
                tv1.setTextColor(Color.WHITE);
                tv1.setGravity(Gravity.CENTER);

                tbrow0.addView(tv1);
                TextView tv2 = new TextView(Inventory.this);
                tv2.setText(" Quantity ");
                tv2.setGravity(Gravity.CENTER);

                tv2.setTextColor(Color.WHITE);
                tbrow0.addView(tv2);
                stk.addView(tbrow0);



                for (int i = 0; i < items.length(); i++) {
                    JSONObject videoObject = items.getJSONObject(i);
                    InventoryClass inc=new InventoryClass(videoObject.getString("IdItem"),videoObject.getString("NameItem"),Integer.parseInt(videoObject.getString("Quantity")));
                //  sl.add(videoObject.getString("IdItem")+"-"+videoObject.getString("NameItem")+"-"+videoObject.getString("Quantity"));
                    Log.d("data","DAta of list json--"+inc.getItemId()+""+ inc.getItemName()+""+inc.getQuantity());
                    //   Toast.makeText(getBaseContext(), "Received!", Toast.LENGTH_LONG).show();
                 //   Toast.makeText(getBaseContext(), inc.getItemId(), Toast.LENGTH_LONG).show();
                    //  etResponse.setText(result);
                    TableRow tbrow = new TableRow(Inventory.this);
                    TextView t1v = new TextView(Inventory.this);
                    t1v.setText(videoObject.getString("IdItem"));
                    t1v.setTextColor(Color.WHITE);
                    t1v.setGravity(Gravity.LEFT);
                    tbrow.addView(t1v);
                    TextView t2v = new TextView(Inventory.this);
                    t2v.setText(videoObject.getString("NameItem"));
                    t2v.setTextColor(Color.WHITE);
                    t2v.setGravity(Gravity.CENTER);
                    tbrow.addView(t2v);
                    TextView t3v = new TextView(Inventory.this);
                    t3v.setText(videoObject.getString("Quantity"));
                    t3v.setTextColor(Color.WHITE);
                    t3v.setGravity(Gravity.RIGHT);
                    tbrow.addView(t3v);
                    stk.addView(tbrow);

                }
              //  ListView view=(ListView) findViewById(R.id.listView);
               // ArrayAdapter<String> adapter = new ArrayAdapter<String>(Inventory.this,R.layout.list_view,sl);
                //view.setAdapter(adapter);



            }catch(JSONException e){
             e.printStackTrace();
                }
                }
        }

    @Override
    protected void onStop() {
        super.onStop();
        finish();

    }


}

