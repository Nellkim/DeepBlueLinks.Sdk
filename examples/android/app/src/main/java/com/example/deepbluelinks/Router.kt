package com.example.deepbluelinks

import android.widget.TextView

// Stands in for your app's own navigation: it shows the route it would open.
class Router(private val view: TextView) {
    fun open(path: String, query: String?) {
        view.text = if (query == null) path else "$path?$query"
    }
}
