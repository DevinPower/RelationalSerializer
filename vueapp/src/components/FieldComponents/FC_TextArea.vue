<template>
    <div class="mt-2">
      <textarea rows="4" class="block w-full shadow-sm rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6" 
        placeholder="..." 
        :value="inputValue"
        spellcheck="true"
        @input="debouncedEmit($event.target.value)" />
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: ['modelValue', 'additionalData', 'selfName'],
        emits :['update:modelValue'],
        data() {
            return {
                inputValue: this.modelValue
            };
        },
        created() {
            this._debounceTimeout = null;
        },
        watch: {
            modelValue(newVal) {
                if (newVal !== this.inputValue) {
                    this.inputValue = newVal;
                }
            }
        },
        methods: {
            debouncedEmit(val) {
                this.inputValue = val;
                if (this._debounceTimeout) clearTimeout(this._debounceTimeout);
                this._debounceTimeout = setTimeout(() => {
                    this.$emit('update:modelValue', val);
                }, 200);
            }
        },
    });
</script>