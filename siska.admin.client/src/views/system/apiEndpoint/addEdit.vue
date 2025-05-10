<template>
    <CRow>
        <CCol :xs="12">
            <CCard class="mb-4">
                <CCardHeader>
                    <strong>{{ mode === 'add' ? 'Add' : 'Edit' }} Api Endpoint</strong>
                </CCardHeader>
                <CCardBody>
                    <CForm @submit.prevent="savePage">
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Path
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.apiPath"
                                            type="text"
                                            :readonly="mode == 'edit'"
                                            :plain-text="mode == 'edit'" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Method
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.apiMethod"
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
                                <CFormInput v-model="pageData.apiDescription"
                                            type="text" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Roles
                            </CFormLabel>
                            <CCol sm="10">
                                <CTable>
                                    <CTableHead>
                                        <CTableRow>
                                            <CTableHeaderCell>Name</CTableHeaderCell>
                                            <CTableHeaderCell>
                                                <CButton color="success" size="sm" @click="showRoleList">
                                                    <CIcon :icon="cilPlus"></CIcon>
                                                </CButton>
                                            </CTableHeaderCell>
                                        </CTableRow>
                                    </CTableHead>
                                    <CTableBody v-for="(role, index) in pageData.roles" :key="role.id">
                                        <CTableRow>
                                            <CTableDataCell>{{ role.name }}</CTableDataCell>
                                            <CTableDataCell>
                                                <CButton color="danger" size="sm" @click="() => { removeRole(index) }">
                                                    <CIcon :icon="cilX"></CIcon>
                                                </CButton>
                                            </CTableDataCell>
                                        </CTableRow>
                                    </CTableBody>
                                </CTable>
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

    <CModal :visible="showList" @close="() => {
            showList = false
        }
        ">
        <CModalHeader>
            <CModalTitle>Role List</CModalTitle>
        </CModalHeader>
        <CModalBody>
            <CInputGroup class="mb-3">
                <CFormInput placeholder="Search by Name" v-model="searchString" />
                <CButton type="button" color="secondary" id="button-addon2" @click="getRoleData">Search</CButton>
            </CInputGroup>
            <CTable>
                <CTableHead color="dark">
                    <CTableRow>
                        <CTableHeaderCell scope="col">#</CTableHeaderCell>
                        <CTableHeaderCell scope="col">Name</CTableHeaderCell>
                        <CTableHeaderCell scope="col">
                        </CTableHeaderCell>
                    </CTableRow>
                </CTableHead>
                <CTableBody v-for="(data, index) in lData" :key="data.id">
                    <CTableRow>
                        <CTableHeaderCell scope="row">
                            {{
                                (currentPage - 1) * itemsPerPage + index + 1
                            }}
                        </CTableHeaderCell>
                        <CTableDataCell>{{ data.name }}</CTableDataCell>
                        <CTableDataCell>
                            <CButton color="success" size="sm" @click="addRole(data)">
                                <CIcon :icon="cilPlus"></CIcon>
                            </CButton>
                        </CTableDataCell>
                    </CTableRow>
                </CTableBody>
            </CTable>
            <Pagination :currentPage="currentPage" :itemsPerPage="itemsPerPage" :totalItems="totalItems"
                        @onPageChange="getRoleData" />
            <CAlert :color="mainMessage.color" :visible="mainMessage.shown" dismissible
                    @close="() => { mainMessage.shown = false }">{{ mainMessage.message }}</CAlert>
        </CModalBody>
        <CModalFooter>
            <CButton color="primary" @click="() => {
                    showList = false
                }
                ">
                Cancel
            </CButton>
        </CModalFooter>
    </CModal>
</template>
<script setup>
    import { ref, onMounted } from 'vue'
    import { useRouter, useRoute } from 'vue-router'
    import { cilPlus, cilX } from '@coreui/icons'
    import Pagination from '@/components/Pagination.vue' // Adjust the import path  

    const router = useRouter()
    const route = useRoute()
    const mode = ref('add') // Set this to 'edit' in the edit mode route

    const pageData = ref({
        id: 0,
        apiPath: '',
        apiMethod: '',
        apiDescription: '',
        roles: []
    })

    const mainMessage = ref({
        message: '',
        shown: false,
        color: 'danger'
    })

    const showList = ref(false)
    const lData = ref()
    const currentPage = ref(1)
    const itemsPerPage = ref(10)
    const totalItems = ref(100)
    const searchString = ref('')

    //#region Role
    const showRoleList = () => {
        getRoleData()
        showList.value = true;
    }

    const addRole = (data) => {
        console.log("add role ", data)

        if (!pageData.value.roles.some(item => item["id"] === data.id))
            pageData.value.roles.push(data)

        showList.value = false;
    }

    const removeRole = (idx) => {
        console.log("remove poultry " + idx)
        pageData.value.roles.splice(idx, 1)
    }

    const getRoleData = () => {

        var dataPost = {
            pageNumber: currentPage.value,
            pageSize: itemsPerPage.value,
        }

        if (searchString.value.length > 0) {
            dataPost = {
                pageNumber: currentPage.value,
                pageSize: itemsPerPage.value,
                search: [
                    { field: "Name", opr: "Contains", value: searchString.value }
                ]
            }
        }

        fetch("api/roles/list", {
            method: "post",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getAccessToken()}`,
            },
            //make sure to serialize your JSON body
            body: JSON.stringify(dataPost)
        })
            .then(result => {
                if (result.ok) {
                    result.json().then(data => {
                        lData.value = data.data
                        totalItems.value = data.totalItems
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
    //#endregion Role

    //#region Main
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
        fetch(`api/apiendpoint?id=${code}`, {
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
                        pageData.value.apiPath = data.apiPath
                        pageData.value.apiMethod = data.apiMethod
                        pageData.value.apiDescription = data.apiDescription
                        pageData.value.roles = data.roles
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
                    router.replace('/sys/apiendpoint').then(() => {
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
                    router.replace('/sys/apiendpoint').then(() => {
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
        return fetch("api/apiendpoint", {
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
        return fetch(`api/apiendpoint`, {
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
    //#endregion Main
</script>
