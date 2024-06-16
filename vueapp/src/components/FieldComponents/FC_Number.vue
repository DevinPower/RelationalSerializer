<template>
    <input :value="modelValue"
           type="number"
           @input="$emit('update:modelValue', $event.target.value)" />
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