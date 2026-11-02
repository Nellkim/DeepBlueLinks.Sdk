package com.example.deepbluelinks

import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.json.JSONObject
import java.io.IOException
import java.net.HttpURLConnection
import java.net.URL

data class ResolvedLink(val id: Int, val longUrl: String, val shortUrl: String, val isSandbox: Boolean)

object DeepBlueLinks {
    suspend fun resolve(url: String): ResolvedLink? =
        post("${Config.API_URL}/api/links/resolve", JSONObject().put("url", url))

    suspend fun post(endpoint: String, body: JSONObject): ResolvedLink? = withContext(Dispatchers.IO) {
        val connection = URL(endpoint).openConnection() as HttpURLConnection
        try {
            connection.requestMethod = "POST"
            connection.doOutput = true
            connection.setRequestProperty("Api-Key", Config.API_KEY)
            connection.setRequestProperty("Content-Type", "application/json")
            connection.outputStream.use { it.write(body.toString().toByteArray()) }
            when (connection.responseCode) {
                200 -> JSONObject(connection.inputStream.bufferedReader().readText()).let {
                    ResolvedLink(it.getInt("id"), it.getString("longUrl"), it.getString("shortUrl"), it.optBoolean("isSandbox"))
                }
                404 -> null
                else -> throw IOException("Deep Blue Links answered ${connection.responseCode}")
            }
        } finally {
            connection.disconnect()
        }
    }
}
