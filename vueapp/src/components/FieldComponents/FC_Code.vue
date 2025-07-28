
<template>
  <CodeMirror
    v-model="codeValue"
    :extensions="[luaLang]"
    basic tab
  />
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import {StreamLanguage} from "@codemirror/language"
    import {lua} from "@codemirror/legacy-modes/mode/lua"
    import CodeMirror from 'vue-codemirror6';

    const luaLang = StreamLanguage.define(lua);

    export default defineComponent({
      props: ['modelValue', 'additionalData', 'selfName'],
      emits: ['update:modelValue'],
      components: {
        CodeMirror
      },
      data() {
        return {
          luaLang,
          codeValue: this.modelValue
        };
      },
      watch: {
        modelValue(newVal) {
          if (newVal !== this.codeValue) {
            this.codeValue = newVal;
          }
        },
        codeValue(newVal) {
          this.debouncedEmit(newVal);
        }
      },
      created() {
        this._debounceTimeout = null;
      },
      methods: {
        debouncedEmit(val) {
          if (this._debounceTimeout) clearTimeout(this._debounceTimeout);
          this._debounceTimeout = setTimeout(() => {
            this.$emit('update:modelValue', val);
          }, 150);
        }
      }
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