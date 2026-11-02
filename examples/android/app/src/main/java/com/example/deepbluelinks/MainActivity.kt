package com.example.deepbluelinks

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.widget.Button
import android.widget.TextView
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.lifecycle.lifecycleScope
import kotlinx.coroutines.CoroutineExceptionHandler
import kotlinx.coroutines.launch

class MainActivity : AppCompatActivity() {
    private lateinit var openedLinkView: TextView
    private lateinit var deferredLinkView: TextView
    private lateinit var lastErrorView: TextView
    private lateinit var router: Router

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_main)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        openedLinkView = findViewById(R.id.opened_link)
        deferredLinkView = findViewById(R.id.deferred_link)
        lastErrorView = findViewById(R.id.last_error)
        router = Router(findViewById(R.id.route))
        findViewById<Button>(R.id.check_deferred_link).setOnClickListener { checkReferrer() }
        openedLinkView.text = getString(R.string.no_link_yet, Config.LINK_DOMAIN, Config.URL_SCHEME)

        handleLink(intent)

        val prefs = getSharedPreferences("deep_blue_links", MODE_PRIVATE)
        if (!prefs.getBoolean("checked_referrer", false)) {
            prefs.edit().putBoolean("checked_referrer", true).apply()
            checkReferrer()
        } else {
            deferredLinkView.setText(R.string.deferred_checked_earlier)
        }
    }

    override fun onNewIntent(intent: Intent) {
        super.onNewIntent(intent)
        setIntent(intent)
        handleLink(intent)
    }

    private fun handleLink(intent: Intent?) {
        val uri = intent?.takeIf { it.action == Intent.ACTION_VIEW }?.data ?: return
        if (uri.scheme == Config.URL_SCHEME) {
            openedLinkView.text = getString(R.string.opened_by_scheme, uri.toString())
            router.open("/" + uri.host + (uri.path ?: ""), uri.query)
            return
        }
        openedLinkView.text = getString(R.string.resolving, uri.toString())
        lifecycleScope.launch(showError(openedLinkView)) {
            val link = DeepBlueLinks.resolve(uri.toString())
            openedLinkView.text = link?.let(::describe) ?: getString(R.string.not_a_short_link, uri.toString())
            val destination = link?.let { Uri.parse(it.longUrl) } ?: uri
            router.open(destination.path ?: "/", destination.query)
        }
    }

    private fun checkReferrer() {
        deferredLinkView.setText(R.string.deferred_checking)
        readReferredLink(this) { link ->
            if (link == null) {
                deferredLinkView.setText(R.string.no_deferred_link)
            } else {
                lifecycleScope.launch(showError(deferredLinkView)) {
                    deferredLinkView.text = getString(R.string.deferred_resolving, link)
                    val resolved = DeepBlueLinks.resolve(link)
                    if (resolved == null) {
                        deferredLinkView.text = getString(R.string.deferred_not_a_short_link, link)
                    } else {
                        deferredLinkView.text = describe(resolved)
                        val destination = Uri.parse(resolved.longUrl)
                        router.open(destination.path ?: "/", destination.query)
                    }
                }
            }
        }
    }

    private fun describe(link: ResolvedLink): String =
        getString(R.string.resolved_link, link.id, link.longUrl, link.shortUrl, link.isSandbox)

    // Without a handler, a failed call (no network, a wrong key) would crash the app.
    private fun showError(resultView: TextView) = CoroutineExceptionHandler { _, error ->
        val message = error.message ?: error.toString()
        resultView.text = getString(R.string.error, message)
        lastErrorView.text = message
    }
}
