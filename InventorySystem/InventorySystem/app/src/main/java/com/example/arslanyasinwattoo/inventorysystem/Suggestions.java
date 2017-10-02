package com.example.arslanyasinwattoo.inventorysystem;

import android.app.Activity;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.annotation.InterpolatorRes;
import android.support.annotation.Nullable;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.widget.ArrayAdapter;
import android.widget.ExpandableListView;
import android.widget.ListView;
import android.widget.Toast;

import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
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
import java.lang.reflect.Array;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

/**
 * Created by Arslanyasinwattoo on 6/29/2016.
 */
public class Suggestions extends AppCompatActivity {
    static List<String> lists= new ArrayList<>();
  //  ExpandableListAdapter listAdapter;
    //ExpandableListView expListView;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.suggestions);
        new HttpAsyncTask().execute("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetRecipes");


    }

    public static String GET(String url){
        InputStream inputStream = null;
        String result = "";
        try {

            // create HttpClient
            HttpClient httpclient = new DefaultHttpClient();

            // make GET request to the given URL
            HttpResponse httpResponse = httpclient.execute(new HttpGet("http://simil.cloudapp.net/SmartFridgeServer/Api/Fridge/GetInventory"));

            // receive response as inputStream
            inputStream = httpResponse.getEntity().getContent();

            // convert inputstream to string
            if(inputStream != null)
                result = convertInputStreamToString(inputStream);
            else
                result = "Did not work!";


          lists= new ArrayList<>();
            try {

                Log.d("Response of GET request", result.toString());
                JSONArray inventroys = new JSONArray(result.toString());
                List<InventoryClass> list= new ArrayList<>();
                // List<String> lists= new ArrayList<>();
                for (int i = 0; i < inventroys.length(); i++) {
                    JSONObject videoObject = inventroys.getJSONObject(i);
                    InventoryClass inc=new InventoryClass(videoObject.getString("IdItem"),videoObject.getString("NameItem"),Integer.parseInt(videoObject.getString("Quantity")));
                   if(Integer.parseInt(videoObject.getString("Quantity"))>0) {
                       lists.add(videoObject.getString("NameItem"));
                   }
                       //  sl.add(videoObject.getString("IdItem")+"-"+videoObject.getString("NameItem")+"-"+videoObject.getString("Quantity"));
                    Log.d("data", "DAta of list json--" + inc.getItemId() + "" + inc.getItemName() + "" + inc.getQuantity());
                    list.add(inc);
                }

            } catch(JSONException e){
                e.printStackTrace();
            }
             httpResponse = httpclient.execute(new HttpGet(url));

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
            //expListView = (ExpandableListView) findViewById(R.id.expandableListView);

            Log.d("Testing", result);
            try {
                Toast.makeText(getBaseContext(),"Recieve", Toast.LENGTH_LONG).show();
                JSONArray items = new JSONArray(result);
                ArrayList<String> parent= new ArrayList<>();
               // HashMap<String, List<String>> childItems = new HashMap<String, List<String>>();
                //childItems= new ArrayList<>();

                for (int i = 0; i < items.length(); i++) {
                    JSONObject videoObject = items.getJSONObject(i);

                    ArrayList<String> child= new ArrayList<>();
                    Log.d("length of object", ""+videoObject.length());

                    Log.d("testing", videoObject.getString("Ingredients"));
                    JSONArray ing = new JSONArray(videoObject.getString("Ingredients"));
                    List<String> list2= new ArrayList<>();
                    for(int j=0; j<ing.length();j++){

                        JSONObject vo = ing.getJSONObject(j);
                        Log.d("testing", vo.getString("IdItem"));
                    //if(vo.getString("IdItem"))
                        //    child.add(vo.getString("IdItem")+":"+vo.getString("DescItem"));
                        list2.add(vo.getString("DescItem"));

                    }
                    if(lists.containsAll(list2)){
                        Log.d("testing", "it is equal ");
                        //parent.add(videoObject.getString("IdRecipe")+":"+videoObject.getString("DescRecipe"));
                        parent.add(videoObject.getString("DescRecipe"));
                    }else{
                        Log.d("testing", "it not equal ");
                    }
                //    ing = new JSONArray(videoObject.getString("ExtraIngredients"));
                  //  for(int j=0; j<ing.length();j++){

                    //    JSONObject vo = ing.getJSONObject(j);
                      //  Log.d("testing", vo.getString("IdItem"));
                       // child.add(vo.getString("IdItem")+":"+vo.getString("DescItem"));
                    //}
                //    childItems.put(parent.get(i),child);
                    // Log.d("data","DAta of list json--"+inc.getItemId()+""+ inc.getItemName()+""+inc.getQuantity());
                }

                // preparing list data


              //  listAdapter = new ExpandableListAdapter(Suggestions.this, parent, childItems);

                // setting list adapter
               // expListView.setAdapter(listAdapter);
                  ListView view=(ListView) findViewById(R.id.listView);
                 ArrayAdapter<String> adapter = new ArrayAdapter<String>(Suggestions.this,R.layout.list_view,parent);
                view.setAdapter(adapter);

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
