<template>
    <div class="infobox">
        <div class="emoji">ℹ️</div>
        <p><b>{{this.modelValue}}</b></p>
    </div>

</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
          props: [ 'modelValue', 'additionalData', 'selfName' ],
          emits :['update:modelValue'],
        data() {
            return {
            };
        },
        created() {
        },
        watch: {
        },
        methods: {
        },
    });
</script>

<style>
    .infobox {
        background-color: #5da3c8;
        color: #fff;
        border: 2px solid #2980b9;
        padding: 15px;
        border-radius: 5px;
        position: relative;
        margin: 20px auto;
    }

    .emoji {
        font-size: 24px;
        position: absolute;
        top: -15px;
        right: -15px;
    }
</style>