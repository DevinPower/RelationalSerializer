<template>
    <select :value="modelValue" @input="$emit('update:modelValue', $event.target.value)">
        <option :value="index" v-for="(property,index) in additionalData.enum.values" :key="property.name">{{property.value}}, {{property.name}}</option>
    </select>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: ['modelValue', 'additionalData', 'selfName'],
        emits: ['update:modelValue'],
        data() {
            return {
            };
        },
        created() {
        },
        watch: {
        },
        methods: {
            check (e) {
              this.$nextTick(() => {
                if (this.modelValue == true)
                    this.$emit('update:modelValue', false);
                else
                    this.$emit('update:modelValue', true);
              });
              
              }
        },
    });
</script>

<style>
    .toggle-switch {
        position: relative;
        display: inline-block;
        width: 60px;
        height: 34px;
    }

        .toggle-switch input {
            opacity: 0;
            width: 0;
            height: 0;
        }

    .slider {
        position: absolute;
        cursor: pointer;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background-color: #ccc;
        -webkit-transition: .4s;
        transition: .4s;
    }

        .slider:before {
            position: absolute;
            content: "";
            height: 26px;
            width: 26px;
            left: 4px;
            bottom: 4px;
            background-color: white;
            -webkit-transition: .4s;
            transition: .4s;
        }

    input:checked + .slider {
        background-color: #2196F3;
    }

    input:focus + .slider {
        box-shadow: 0 0 1px #2196F3;
    }

    input:checked + .slider:before {
        -webkit-transform: translateX(26px);
        -ms-transform: translateX(26px);
        transform: translateX(26px);
    }

    .slider.round {
        border-radius: 34px;
    }

        .slider.round:before {
            border-radius: 50%;
        }
</style>