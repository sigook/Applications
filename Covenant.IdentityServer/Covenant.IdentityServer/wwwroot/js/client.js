class Client {
    static RedirectUris = [];
    static PostLogoutRedirectUris = [];
    static AllowedCorsOrigins = [];

    static LoadData(list, listId, data) {
        this[list] = [... data];
        this.#AddElementToList(list, listId);
    }
    
    static AddItem(event, inputId, list, listId) {
        if (event.key === "Enter") {
            let val = document.querySelector(`${inputId}`).value;
            if (val !== '') {
                if (!(this[list].indexOf(val) >= 0)) {
                    this[list].push(val);
                    this.#AddElementToList(list, listId);
                    document.querySelector(`${inputId}`).value = '';
                }
                else {
                    document.querySelector(`${inputId}`).value = '';
                }
            }
            event.preventDefault();
        }
    }

    static #AddElementToList(listName, listId) {
        const list = document.querySelector(`${listId}`);
        list.innerHTML = '';
        this[listName].map((item, index) => {
            const li = document.createElement("li");
            const span = document.createElement("span");
            const a = document.createElement("a");
            span.innerHTML = item;
            a.onclick = () => this.#RemoveItem(listName, listId, index);
            a.innerHTML = "X";
            li.appendChild(span);
            li.appendChild(a);
            list.appendChild(li);
        });
    }

    static #RemoveItem(listName, listId, index) {
        this[listName] = this[listName].filter(item => this[listName].indexOf(item) != index);
        this.#AddElementToList(listName, listId);
    }
}

document.addEventListener("DOMContentLoaded", () => {
    document.querySelector("#redirectUri")
        .addEventListener("keypress", (e) => Client.AddItem(e, "#redirectUri", "RedirectUris", "#redirectUrisList"));
    document.querySelector("#postLogoutRedirectUri")
        .addEventListener("keypress", (e) => Client.AddItem(e, "#postLogoutRedirectUri", "PostLogoutRedirectUris", "#postLogoutRedirectUrisList"));
    document.querySelector("#allowedCorsOrigin")
        .addEventListener("keypress", (e) => Client.AddItem(e, "#allowedCorsOrigin", "AllowedCorsOrigins", "#allowedCorsOriginList"));
    
});
