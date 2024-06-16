<template>
    <CodeEditor @input="$emit('update:modelValue', $event.target.value)"
                :value="modelValue"

                :languages="[[additionalData.language, additionalData.language]]"
                style="width:100%"
                :theme="additionalData.codeEditorTheme" />

</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import CodeEditor from 'simple-code-editor';

    export default defineComponent({
        props: ['modelValue', 'additionalData', 'selfName'],
          emits :['update:modelValue'],
        data() {
            return {
            };
        },
        components: {
            CodeEditor
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