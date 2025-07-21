<template>
    <div class="border-b border-gray-200 pb-px focus-within:border-b-2 focus-within:border-indigo-600 focus-within:pb-0">
          <label for="comment" class="sr-only">Add your comment</label>
          <input rows="3" name="comment" id="comment" class="block w-full resize-none text-base text-gray-900 placeholder:text-gray-400 focus:outline-none sm:text-sm/6" 
            placeholder="Add your comment..." 
            :value="modelValue"
            @input="$emit('update:modelValue', $event.target.value)" />
        </div>
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