<script setup>
  import { ref, onMounted } from 'vue'
  import { useRouter, useRoute } from 'vue-router'

  const router = useRouter()
  const avatar = ref('')
  const avatarUserName = ref('')
  const itemsCount = 42

  onMounted(() => {
    const storage = sessionStorage.getItem('user')
    let user = {}

    if (storage) {
      user = JSON.parse(storage)

      avatarUserName.value = generateAvatarStringFromName(user.name)

      getUserImage(user.id, user.token)
    }
  })

  const generateAvatarStringFromName = (fullName) => {
    const nameParts = fullName.split(' ');

    let initials = nameParts[0].slice(0, 2)
    if (nameParts.length > 1) {
      initials = nameParts[0].charAt(0) + nameParts[nameParts.length - 1].charAt(0)
    }

    return initials;
  }

  const getUserImage = (id, token) => {
    fetch(`api/users/image?id=${id}`, {
      method: "get",
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      }
    })
      .then(result => {
        if (result.ok) {
          result.blob().then(blob => {
            if (blob && blob.size > 0)
              avatar.value = window.URL.createObjectURL(blob);
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

</script>

<template>
  <CDropdown placement="bottom-end" variant="nav-item">
    <CDropdownToggle class="py-0 pe-0" :caret="false">
      <CAvatar :src="avatar" size="md" color="success">{{ avatarUserName }}</CAvatar>
    </CDropdownToggle>
    <CDropdownMenu class="pt-0">
      <CDropdownHeader
        component="h6"
        class="bg-body-secondary text-body-secondary fw-semibold mb-2 rounded-top"
      >
        Account
      </CDropdownHeader>
      <CDropdownItem>
        <CIcon icon="cil-bell" /> Updates
        <CBadge color="info" class="ms-auto">{{ itemsCount }}</CBadge>
      </CDropdownItem>
      <CDropdownItem>
        <CIcon icon="cil-envelope-open" /> Messages
        <CBadge color="success" class="ms-auto">{{ itemsCount }}</CBadge>
      </CDropdownItem>
      <CDropdownItem>
        <CIcon icon="cil-task" /> Tasks
        <CBadge color="danger" class="ms-auto">{{ itemsCount }}</CBadge>
      </CDropdownItem>
      <CDropdownItem>
        <CIcon icon="cil-comment-square" /> Comments
        <CBadge color="warning" class="ms-auto">{{ itemsCount }}</CBadge>
      </CDropdownItem>
      <CDropdownHeader
        component="h6"
        class="bg-body-secondary text-body-secondary fw-semibold my-2"
      >
        Settings
      </CDropdownHeader>
      <CDropdownItem @click="() => {router.push('/sys/users/profile')}"> <CIcon icon="cil-user" /> Profile </CDropdownItem>
      <CDropdownItem> <CIcon icon="cil-settings" /> Settings </CDropdownItem>
      <CDropdownItem>
        <CIcon icon="cil-dollar" /> Payments
        <CBadge color="secondary" class="ms-auto">{{ itemsCount }}</CBadge>
      </CDropdownItem>
      <CDropdownItem>
        <CIcon icon="cil-file" /> Projects
        <CBadge color="primary" class="ms-auto">{{ itemsCount }}</CBadge>
      </CDropdownItem>
      <CDropdownDivider />
      <CDropdownItem> <CIcon icon="cil-shield-alt" /> Lock Account </CDropdownItem>
      <CDropdownItem> <CIcon icon="cil-lock-locked" /> Logout </CDropdownItem>
    </CDropdownMenu>
  </CDropdown>
</template>
