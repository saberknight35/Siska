<template>
    <CRow>
        <CCol :xs="12">
            <CCard class="mb-4">
                <CCardHeader>
                    <strong>{{ mode === 'add' ? 'Add' : 'Edit' }} Role</strong>
                </CCardHeader>
                <CCardBody>
                    <CForm @submit.prevent="savePage">
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Name
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.name"
                                            type="text"
                                            :readonly="mode == 'edit'"
                                            :plain-text="mode == 'edit'" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Description
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.description"
                                            type="text" />
                            </CCol>
                        </CRow>
                        <CAlert :color="mainMessage.color" :visible="mainMessage.shown" dismissible
                                @close="() => { mainMessage.shown = false }">{{ mainMessage.message }}</CAlert>

                        <CButton color="primary" type="submit">
                            {{
              mode === 'add' ? 'Add' : 'Save'
                            }}
                        </CButton>
                        &nbsp;
                        <CButton color="warning" @click="cancel">Cancel</CButton>
                    </CForm>
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>
</template>
<script setup>
    import { ref, onMounted } from 'vue'
    import { useRouter, useRoute } from 'vue-router'

    const router = useRouter()
    const route = useRoute()
    const mode = ref('add') // Set this to 'edit' in the edit mode route

    const pageData = ref({
        id: '',
        name: '',
        description: '',
    })

    const resetPassword = ref(false)
    const newPassword = ref('')

    const mainMessage = ref({
        message: '',
        shown: false,
        color: 'danger'
    })

    const getAccessToken = () => {
        // const storage = localStorage.getItem("user")
        const storage = sessionStorage.getItem('user')
        if (storage) {
            let user = JSON.parse(storage)

            return user.token
        }

        return null
    }

    const fetchPageData = (code) => {
        fetch(`api/roles?id=${code}`, {
            method: "get",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getAccessToken()}`,
            }
        })
            .then(result => {
                if (result.ok) {
                    result.json().then(data => {
                        console.log("data get ==> ", data)
                        pageData.value.id = data.id
                        pageData.value.name = data.name
                        pageData.value.description = data.description
                    })
                } else {
                    result.json().then(data => {
                        console.log("error get ==> ", data)
                        mainMessage.value.shown = true
                        mainMessage.value.message = data.message
                        mainMessage.value.color = 'danger'
                    })
                }
            })
    }

    onMounted(() => {
        // Check if the component is in edit mode based on the route parameter
        const pageId = route.params.id // Adjust the route parameter name if needed
        if (pageId) {
            mode.value = 'edit'
            // Fetch page data using the pageId and update pageData
            fetchPageData(pageId)
        }
    })

    const savePage = () => {
        if (mode.value === 'add') {
            // Save new page using pageData
            saveNewPage().then(result => {
                if (result.ok) {
                    console.log('addd ==> ', result)
                    router.replace('/sys/roles').then(() => {
                        window.location.reload()
                    })
                } else {
                    result.json().then(data => {
                        console.log("error get ==> ", data)
                        mainMessage.value.shown = true
                        mainMessage.value.message = data.message
                        mainMessage.value.color = 'danger'
                    })
                }
            })
        } else {
            // Update existing page using pageData
            updatePage().then(result => {
                if (result.ok) {
                    console.log('edit ==> ', result)
                    router.replace('/sys/roles').then(() => {
                        window.location.reload()
                    })
                } else {
                    result.json().then(data => {
                        console.log("error get ==> ", data)
                        mainMessage.value.shown = true
                        mainMessage.value.message = data.message
                        mainMessage.value.color = 'danger'
                    })
                }
            })
        }
    }

    const saveNewPage = () => {
        return fetch("api/roles", {
            method: "post",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getAccessToken()}`,
            },
            body: JSON.stringify(pageData.value)
        })
    }

    const updatePage = () => {
        return fetch(`api/roles`, {
            method: "put",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getAccessToken()}`,
            },
            body: JSON.stringify(pageData.value)
        })
    }

    const cancel = () => {
        router.go(-1) // Redirect to the list of pages without saving
    }
</script>
