<template>
    <CRow>
        <CCol :xs="12">
            <CCard class="mb-4">
                <CCardHeader>
                    <strong>API Endpoint</strong>
                </CCardHeader>
                <CCardBody>
                    <CTable>
                        <CTableHead color="dark">
                            <CTableRow>
                                <CTableHeaderCell scope="col">#</CTableHeaderCell>
                                <CTableHeaderCell scope="col">Path</CTableHeaderCell>
                                <CTableHeaderCell scope="col">Method</CTableHeaderCell>
                                <CTableHeaderCell scope="col">Description</CTableHeaderCell>
                                <CTableHeaderCell scope="col">
                                    <CButton color="success" size="sm" @click="addData">
                                        <CIcon :icon="cilPlus"></CIcon>
                                    </CButton>
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
                                <CTableDataCell>{{ data.apiPath }}</CTableDataCell>
                                <CTableDataCell>{{ data.apiMethod }}</CTableDataCell>
                                <CTableDataCell>{{ data.apiDescription }}</CTableDataCell>
                                <CTableDataCell>
                                    <CButton color="warning"
                                             size="sm"
                                             @click="editData(data.id)">
                                        <CIcon :icon="cilPencil"></CIcon>
                                    </CButton>
                                    &nbsp;
                                    <CButton color="danger"
                                             size="sm"
                                             @click="
                                             ()=>
                                        {
                                        deleteConfirmation = true
                                        deletedDataId = data.id
                                        }
                                        "
                                        >
                                        <CIcon :icon="cilX"></CIcon>
                                    </CButton>
                                </CTableDataCell>
                            </CTableRow>
                        </CTableBody>
                    </CTable>
                    <Pagination :currentPage="currentPage"
                                :itemsPerPage="itemsPerPage"
                                :totalItems="totalItems"
                                @onPageChange="goToPage" />
                    <CAlert :color="mainMessage.color" :visible="mainMessage.shown" dismissible
                            @close="() => { mainMessage.shown = false }">{{ mainMessage.message }}</CAlert>
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>
    <CModal :visible="deleteConfirmation"
            @close="
            ()=>
        {
        deleteConfirmation = false
        }
        "
        >
        <CModalHeader>
            <CModalTitle>Data Status Change Confirmation</CModalTitle>
        </CModalHeader>
        <CModalBody>Are you sure you want to change status of this data?</CModalBody>
        <CModalFooter>
            <CButton color="secondary" @click="deleteData()">Yes</CButton>
            <CButton color="primary"
                     @click="
                     ()=>
                {
                deleteConfirmation = false
                }
                "
                >
                Cancel
            </CButton>
        </CModalFooter>
    </CModal>
</template>
<script setup>
    import { ref, onMounted } from 'vue'
    import Pagination from '@/components/Pagination.vue' // Adjust the import path
    import { cilPlus, cilPencil, cilX } from '@coreui/icons'
    import { useRouter } from 'vue-router'

    const router = useRouter()

    const lData = ref()
    const currentPage = ref(1)
    const itemsPerPage = ref(10)
    const totalItems = ref(100)

    const deleteConfirmation = ref(false)
    const deletedDataId = ref('')

    const mainMessage = ref({
        message: '',
        shown: false,
        color: 'danger'
    })

    const goToPage = (pageNumber) => {
        currentPage.value = pageNumber
        getData()
    }

    const getAccessToken = () => {
        // const storage = localStorage.getItem("user")
        const storage = sessionStorage.getItem('user')

        if (storage) {
            let user = JSON.parse(storage)
            return user.token
        }

        return null
    }

    const getData = () => {

        fetch("api/apiendpoint/list", {
            method: "post",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getAccessToken()}`,
            },
            //make sure to serialize your JSON body
            body: JSON.stringify({
                pageNumber: currentPage.value,
                pageSize: itemsPerPage.value,
            })
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

    const addData = () => {
        console.log('add')
        router.push('/sys/apiendpoint/add')
    }

    const editData = (id) => {
        console.log('edit => ', id)
        router.push('/sys/apiendpoint/edit/' + id)
    }

    const deleteData = () => {
        console.log('delete => ', deletedDataId.value)

        fetch(`api/apiendpoint?id=${deletedDataId.value}`, {
            method: "delete",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getAccessToken()}`,
            }
        })
            .then(result => {
                if (result.ok) {
                    result.json().then(data => {
                        deleteConfirmation.value = false
                        currentPage.value = 1
                        getData()
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

    onMounted(getData)
</script>
