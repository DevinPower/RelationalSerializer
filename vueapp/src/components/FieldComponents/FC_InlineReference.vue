<template>
    <div>
        <div v-if="modelValue">
            <WebObject
            :id="modelValue"
            :project="additionalData.referenceCategory"
            isInline="true"></WebObject>
        </div>
        <div v-else>
            <button @click="instantiate()">Instantiate</button>
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent, defineAsyncComponent } from 'vue';

    export default defineComponent({
        props: ['modelValue', 'additionalData', 'selfName'],
        emits :['update:modelValue'],
        data() {
            return {
            };
        },
        components: {
            "WebObject": defineAsyncComponent(() => import('../WebObject.vue'))
        },
        created() {
        },
        watch: {
        },
        methods: {
            instantiate(){
                this.$parent.instantiateInlineReference(this.selfName);
            }
        },
    });
</script>

<style>
</style>