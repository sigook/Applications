<template>
    <div>
        <h2 class="bg-dark">{{ $t("ReplaceWorker")}}</h2>
        <div class="contain-new-worker">
            <p>{{ $t("NameWorker")}}</p>
            <input type="text" class="input-block input-border" title="name worker">
            <p>{{ $t("Comment") }}</p>
            <textarea title="comment"></textarea>
           <!-- <button class="background-btn create-btn primary-button btn-radius">Send</button>-->
        </div>
    </div>
</template>
