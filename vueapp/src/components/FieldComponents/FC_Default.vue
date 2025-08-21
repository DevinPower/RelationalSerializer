<template>
    <div class="border-b bg-white border-gray-200 shadow-sm p-1 pb-px focus-within:border-b-2 focus-within:border-indigo-600 focus-within:pb-0">
          <input rows="3" class="block w-full resize-none text-base text-gray-900 placeholder:text-gray-400 focus:outline-none sm:text-sm/6" 
            placeholder="..." 
            :value="inputValue"
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
                }, 250);
            }
        },
    });
</script>