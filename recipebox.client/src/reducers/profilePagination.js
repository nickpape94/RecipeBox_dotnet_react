import { GET_PROFILE_PAGINATION_HEADERS, RESET_PROFILE_PAGINATION_HEADERS } from '../actions/types';

const initialState = {
	loading: true,
	error: {},
	currentPage: null,
	itemsPerPage: null,
	totalItems: null,
	totalPages: null
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case GET_PROFILE_PAGINATION_HEADERS:
			return {
				...state,
				loading: false,
				currentPage: payload.currentPage,
				itemsPerPage: payload.itemsPerPage,
				totalItems: payload.totalItems,
				totalPages: payload.totalPages
			};
		case RESET_PROFILE_PAGINATION_HEADERS:
			return {
				loading: true,
				error: {},
				currentPage: null,
				itemsPerPage: null,
				totalItems: null,
				totalPages: null
			};
		default:
			return state;
	}
}
