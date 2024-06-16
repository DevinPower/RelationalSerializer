<template>
    <input :value="modelValue"
           type="range"
           :min="this.additionalData.minimum"
           :max="this.additionalData.maximum"
           @input="$emit('update:modelValue', $event.target.value)" />
    {{this.modelValue}}
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: ['modelValue', 'additionalData', 'selfName'],
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