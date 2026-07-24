function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for(var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1, c.length);
        }
        if (c.indexOf(nameEQ) == 0) {
            return c.substring(nameEQ.length, c.length);
        }
    }
    return null;
}

function removeCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999; path=/';
}
async function fetchWithCredentials(url, method, body) {
    const options = {
        method: method || 'GET',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' }
    };

    if (body !== undefined && body !== null) {
        options.body = JSON.stringify(body);
    }

    const resp = await fetch(url, options);
    const text = await resp.text();
    return { ok: resp.ok, status: resp.status, content: text };
}