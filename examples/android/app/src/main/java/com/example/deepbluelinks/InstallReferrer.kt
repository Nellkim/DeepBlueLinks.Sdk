package com.example.deepbluelinks

import android.content.Context
import android.util.Log
import com.android.installreferrer.api.InstallReferrerClient
import com.android.installreferrer.api.InstallReferrerStateListener
import java.net.URLDecoder

fun readReferredLink(context: Context, onResult: (String?) -> Unit) {
    val client = InstallReferrerClient.newBuilder(context).build()
    client.startConnection(object : InstallReferrerStateListener {
        override fun onInstallReferrerSetupFinished(responseCode: Int) {
            val link = try {
                if (responseCode == InstallReferrerClient.InstallReferrerResponse.OK) {
                    linkFromReferrer(client.installReferrer.installReferrer)
                } else null
            } catch (e: Exception) {
                Log.w("DeepBlueLinks", "Couldn't read the install referrer", e)
                null
            } finally {
                client.endConnection()
            }
            onResult(link)
        }

        override fun onInstallReferrerServiceDisconnected() {}
    })
}

fun linkFromReferrer(referrer: String?): String? =
    referrer?.split('&')
        ?.map { it.split('=', limit = 2) }
        ?.firstOrNull { it.first() == "link" }
        ?.getOrNull(1)
        ?.let { URLDecoder.decode(it, "UTF-8") }
