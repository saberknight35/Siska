<template>
  <CPagination aria-label="Page navigation example">
    <CPaginationItem
      v-for="pageItem in visiblePageItems"
      :key="pageItem.key"
      :active="pageItem.active"
      @click="goToPage(pageItem.pageNumber)"
    >
      {{ pageItem.content }}
    </CPaginationItem>
  </CPagination>
</template>
<script setup>
import { computed } from 'vue'

const props = defineProps({
  currentPage: Number,
  itemsPerPage: Number,
  totalItems: Number,
})

const events = defineEmits(['onPageChange'])

const totalPages = computed(() =>
  Math.ceil(props.totalItems / props.itemsPerPage),
)

const visiblePageItems = computed(() => {
  const maxVisiblePages = 3
  const visibleItems = []

  let startPage = Math.max(
    props.currentPage - Math.floor(maxVisiblePages / 2),
    1,
  )
  const endPage = Math.min(startPage + maxVisiblePages - 1, totalPages.value)

  if (totalPages.value > maxVisiblePages) {
    if (startPage > 1) {
      visibleItems.push({
        content: '1',
        pageNumber: 1,
        active: false,
        key: 'first',
      })
      if (startPage > 2) {
        visibleItems.push({
          content: '...',
          pageNumber: null,
          active: false,
          key: 'ellipsisStart',
        })
      }
    }

    for (let page = startPage; page <= endPage; page++) {
      visibleItems.push({
        content: page.toString(),
        pageNumber: page,
        active: page === props.currentPage,
        key: page,
      })
    }

    if (endPage < totalPages.value) {
      if (endPage < totalPages.value - 1) {
        visibleItems.push({
          content: '...',
          pageNumber: null,
          active: false,
          key: 'ellipsisEnd',
        })
      }
      visibleItems.push({
        content: totalPages.value.toString(),
        pageNumber: totalPages.value,
        active: false,
        key: 'last',
      })
    }
  } else {
    for (let page = 1; page <= totalPages.value; page++) {
      visibleItems.push({
        content: page.toString(),
        pageNumber: page,
        active: page === props.currentPage,
        key: page,
      })
    }
  }

  return visibleItems
})

const goToPage = (pageNumber) => {
  events('onPageChange', pageNumber)
}
</script>
