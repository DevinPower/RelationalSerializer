<template>
    <div class="modal-mask" style="height:100%">
        <div class="modal-wrapper" style="min-height: 300px;">
            <div class="modal-container" style="height:100%;">
                <h3>{{this.Header}}</h3>
                <p>{{this.Description}}</p>
                <button @click="$emit('confirm')">Confirm</button>
                <button @click="$emit('cancel')">Cancel</button>
            </div>
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: ['Header', 'Description'],
          emits :['update:modelValue'],
        data() {
            return {
            };
        },
        components: {
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
    .modal-container{
        color:black;
    }
</style>