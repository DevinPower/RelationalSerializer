<template>
    <input @click="promptReference(additionalData.referenceCategory)" :value="modelValue" type="text" style="cursor:pointer;" readonly placeholder="null" />

    <ReferenceModal v-if="referencePrompt"
                       :Header="referencePrompt.header"
                       :project="referencePrompt.project"
                       @confirm="referencePrompt.confirmCallback($event)"
                       @cancel="referencePrompt.cancelCallback()" />
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import ReferenceModal from '../ReferenceModal.vue'

    export default defineComponent({
        props: ['modelValue', 'additionalData', 'selfName'],
        emits :['update:modelValue'],
        data() {
            return {
                referencePrompt: null
            };
        },
        components: {
            ReferenceModal
        },
        created() {
        },
        watch: {
        },
        methods: {
            promptReference(assignProject) {
                const thisRef = this;
                this.referencePrompt = {
                    header: "Pick an object to link.",
                    project: assignProject,
                    confirmCallback: function (guid) {
                        thisRef.referencePrompt = null;
                        //thisRef.modelValue = guid;
                        thisRef.$emit('update:modelValue', guid);
                    },
                    cancelCallback: function () {
                        thisRef.referencePrompt = null;
                    }
                }
            }
        },
    });
</script>

<style>
</style>